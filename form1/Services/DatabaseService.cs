using MySqlConnector;
using System.Text.RegularExpressions;
using System.Text;

namespace form.Services
{
    public class DatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public List<DatabaseTarget> GetConfiguredTargets()
        {
            var connectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren();

            return connectionStrings
                .Where(section => !string.IsNullOrWhiteSpace(section.Value))
                .Select(section => CreateTargetFromConnectionString(section.Key, section.Value!))
                .ToList();
        }

        public DatabaseTarget CreateCustomTarget(string name, string host, int port, string database, string user, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host.Trim(),
                Port = (uint)(port > 0 ? port : 3306),
                Database = database.Trim(),
                UserID = user.Trim(),
                Password = password ?? "",
                ConnectionProtocol = MySqlConnectionProtocol.Tcp,
                SslMode = MySqlSslMode.None,
                AllowPublicKeyRetrieval = true,
                ConnectionTimeout = 10
            };

            var displayName = string.IsNullOrWhiteSpace(name)
                ? $"{builder.Database} ({builder.Server}:{builder.Port})"
                : name.Trim();

            return new DatabaseTarget(displayName, builder.ConnectionString);
        }

        public async Task<ExecutionResult> ExecuteQueriesAsync(string sqlContent, IEnumerable<DatabaseTarget> targets)
        {
            var result = new ExecutionResult();

            try
            {
                var queries = ParseSqlQueries(sqlContent);

                if (queries.Count == 0)
                {
                    result.HasError = true;
                    result.Message = "No valid SQL queries found in the file.";
                    return result;
                }

                result.TotalQueries = queries.Count;

                foreach (var target in targets)
                {
                    var databaseResult = await ExecuteQueriesOnDatabase(target.ConnectionString, queries, target.Name);
                    result.DatabaseResults.Add(databaseResult);
                    if (!string.IsNullOrEmpty(databaseResult.Error) || databaseResult.FailedQueries > 0)
                    {
                        result.HasError = true;
                    }
                }

                if (!result.HasError)
                {
                    result.Message = "All queries executed successfully!";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = $"Fatal error: {ex.Message}";
                _logger.LogError(ex, "Error executing queries");
            }

            return result;
        }

        private DatabaseTarget CreateTargetFromConnectionString(string key, string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                var name = $"{key} ({builder.Server}:{builder.Port}/{builder.Database})";
                return new DatabaseTarget(name, connectionString, key);
            }
            catch
            {
                return new DatabaseTarget(key, connectionString, key);
            }
        }

        private async Task<DatabaseExecutionResult> ExecuteQueriesOnDatabase(string connectionString, List<string> queries, string dbName)
        {
            var result = new DatabaseExecutionResult { DatabaseName = dbName };

            try
            {
                using (var connection = await OpenConnectionAsync(connectionString))
                {
                    result.Connected = true;

                    foreach (var query in queries)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(query))
                                continue;

                            using (var command = new MySqlCommand(query, connection))
                            {
                                command.CommandTimeout = 300;
                                var rowsAffected = await command.ExecuteNonQueryAsync();
                                result.SuccessfulQueries++;
                                result.Details.Add($"Query executed successfully (Rows affected: {rowsAffected})");
                            }
                        }
                        catch (MySqlException ex) when (ex.Number == 1062)
                        {
                            result.Details.Add($" Duplicate entry: {ex.Message}");
                            result.DuplicateErrors++;
                        }
                        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.ParseError)
                        {
                            result.Details.Add($" Syntax Error: {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (MySqlException ex) when (ex.Number == 1054)
                        {
                            result.Details.Add($" Unknown column: {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (MySqlException ex) when (
                            ex.ErrorCode == MySqlErrorCode.StoredProcedureDoesNotExist ||
                            ex.ErrorCode == MySqlErrorCode.UnknownProcedure ||
                            ex.ErrorCode == MySqlErrorCode.FunctionNotDefined)
                        {
                            result.Details.Add($" {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("DELIMITER") || ex.Message.Contains("delimiter"))
                            {
                                result.Details.Add($" DELIMITER statement skipped (MySqlConnector limitation)");
                                continue;
                            }

                            result.Details.Add($" Error: {ex.Message}");
                            result.FailedQueries++;
                        }
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 0)
            {
                result.Error = $"Cannot connect to {dbName}: {ex.Message}";
                result.Connected = false;
                _logger.LogError($"Connection error for {dbName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                result.Error = $"Error with {dbName}: {ex.Message}";
                _logger.LogError(ex, $"Error executing on {dbName}");
            }

            return result;
        }

        private async Task<MySqlConnection> OpenConnectionAsync(string connectionString)
        {
            var connection = new MySqlConnection(connectionString);

            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (MySqlException ex) when (ex.Number == 1049)
            {
                await connection.DisposeAsync();

                var builder = new MySqlConnectionStringBuilder(connectionString);
                var databaseName = builder.Database;

                if (string.IsNullOrWhiteSpace(databaseName))
                {
                    throw;
                }

                builder.Database = "";
                await using (var serverConnection = new MySqlConnection(builder.ConnectionString))
                {
                    await serverConnection.OpenAsync();
                    var quotedDatabaseName = QuoteIdentifier(databaseName);
                    await using var createCommand = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {quotedDatabaseName};", serverConnection);
                    await createCommand.ExecuteNonQueryAsync();
                }

                connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                return connection;
            }
        }

        private static string QuoteIdentifier(string identifier)
        {
            return $"`{identifier.Replace("`", "``")}`";
        }

        private List<string> ParseSqlQueries(string sqlContent)
        {
            var queries = new List<string>();

            sqlContent = RemoveComments(sqlContent);

            sqlContent = Regex.Replace(sqlContent, @"\r\n|\r|\n", "\n");

            var delimiter = ";";
            var currentQuery = new StringBuilder();
            var lines = sqlContent.Split('\n');
            var inStoredObject = false;
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                if (trimmedLine.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        delimiter = parts[1];
                    }
                    continue;
                }

                var upperLine = trimmedLine.ToUpper();
                if (!inStoredObject && (upperLine.Contains("CREATE PROCEDURE") || 
                                       upperLine.Contains("CREATE FUNCTION") ||
                                       upperLine.Contains("CREATE TRIGGER") ||
                                       upperLine.Contains("CREATE EVENT")))
                {
                    inStoredObject = true;
                }
                else if (!inStoredObject && (upperLine.StartsWith("DROP PROCEDURE") || 
                                            upperLine.StartsWith("DROP FUNCTION") ||
                                            upperLine.StartsWith("DROP TRIGGER") ||
                                            upperLine.StartsWith("DROP EVENT")))
                {
                    var dropQuery = trimmedLine;
                    if (dropQuery.EndsWith(delimiter))
                    {
                        dropQuery = dropQuery.Substring(0, dropQuery.Length - delimiter.Length);
                    }
                    if (!string.IsNullOrWhiteSpace(dropQuery))
                    {
                        queries.Add(dropQuery.Trim());
                    }
                    delimiter = ";";
                    continue;
                }

                if (currentQuery.Length > 0)
                    currentQuery.Append("\n");
                currentQuery.Append(line);

                bool isEndOfQuery = false;

                if (inStoredObject)
                {
                    if (upperLine.StartsWith("END") && trimmedLine.EndsWith(delimiter))
                    {
                        isEndOfQuery = true;
                        inStoredObject = false;
                    }
                }
                else
                {
                    if (trimmedLine.EndsWith(delimiter))
                    {
                        isEndOfQuery = true;
                    }
                }

                if (isEndOfQuery)
                {
                    var query = currentQuery.ToString().Trim();

                    if (query.EndsWith(delimiter))
                    {
                        query = query.Substring(0, query.Length - delimiter.Length);
                    }

                    query = query.Trim();

                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        queries.Add(query);
                    }

                    currentQuery.Clear();
                    delimiter = ";";
                }
            }

            if (currentQuery.Length > 0)
            {
                var query = currentQuery.ToString().Trim();
                if (query.EndsWith(delimiter))
                {
                    query = query.Substring(0, query.Length - delimiter.Length);
                }
                query = query.Trim();
                if (!string.IsNullOrWhiteSpace(query))
                {
                    queries.Add(query);
                }
            }

            return queries;
        }

        private string RemoveComments(string sql)
        {
            sql = Regex.Replace(sql, @"--.*?(?=\n|$)", "", RegexOptions.Multiline);

            sql = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);

            sql = Regex.Replace(sql, @"#.*?(?=\n|$)", "", RegexOptions.Multiline);

            return sql;
        }

        public async Task<bool> TestConnectionAsync(string connectionString)
        {
            try
            {
                using (var connection = await OpenConnectionAsync(connectionString))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                return false;
            }
        }
    }

    public record DatabaseTarget(string Name, string ConnectionString, string Key = "");

    public class DatabaseTargetStatus
    {
        public DatabaseTarget Target { get; set; } = new("", "");
        public bool Connected { get; set; }
    }

    public class ExecutionResult
    {
        public bool HasError { get; set; }
        public string Message { get; set; } = "";
        public int TotalQueries { get; set; }
        public List<DatabaseExecutionResult> DatabaseResults { get; set; } = new();
    }

    public class DatabaseExecutionResult
    {
        public string DatabaseName { get; set; } = "";
        public bool Connected { get; set; }
        public string Error { get; set; } = "";
        public int SuccessfulQueries { get; set; }
        public int FailedQueries { get; set; }
        public int DuplicateErrors { get; set; }
        public List<string> Details { get; set; } = new List<string>();
    }
}
