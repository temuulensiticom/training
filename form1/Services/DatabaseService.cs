using MySqlConnector;
using System.Text.RegularExpressions;
using System.Text;

namespace form.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString1;
        private readonly string _connectionString2;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _connectionString1 = configuration.GetConnectionString("Database1") ?? "";
            _connectionString2 = configuration.GetConnectionString("Database2") ?? "";
            _logger = logger;
        }

        public async Task<ExecutionResult> ExecuteQueriesAsync(string sqlContent, bool executeOnDb1 = true, bool executeOnDb2 = true)
        {
            var result = new ExecutionResult();

            try
            {
                // Split SQL file into individual queries
                var queries = ParseSqlQueries(sqlContent);

                if (queries.Count == 0)
                {
                    result.HasError = true;
                    result.Message = "No valid SQL queries found in the file.";
                    return result;
                }

                result.TotalQueries = queries.Count;

                if (executeOnDb1)
                {
                    var db1Result = await ExecuteQueriesOnDatabase(_connectionString1, queries, "Database 1 (Port 3306)");
                    result.Database1Results = db1Result;
                    if (!string.IsNullOrEmpty(db1Result.Error))
                    {
                        result.HasError = true;
                    }
                }

                if (executeOnDb2)
                {
                    var db2Result = await ExecuteQueriesOnDatabase(_connectionString2, queries, "Database 2 (Port 3307)");
                    result.Database2Results = db2Result;
                    if (!string.IsNullOrEmpty(db2Result.Error))
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
                            // Skip empty queries
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
                            // Duplicate entry error - warning only
                            result.Details.Add($" Duplicate entry: {ex.Message}");
                            result.DuplicateErrors++;
                        }
                        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.ParseError)
                        {
                            // Syntax errors
                            result.Details.Add($" Syntax Error: {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (MySqlException ex) when (ex.Number == 1054)
                        {
                            // Unknown column error
                            result.Details.Add($" Unknown column: {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (MySqlException ex) when (
                            ex.ErrorCode == MySqlErrorCode.StoredProcedureDoesNotExist ||
                            ex.ErrorCode == MySqlErrorCode.UnknownProcedure ||
                            ex.ErrorCode == MySqlErrorCode.FunctionNotDefined)
                        {
                            // Procedure/Function doesn't exist - might be CALL before CREATE
                            result.Details.Add($" {ex.Message}");
                            result.FailedQueries++;
                        }
                        catch (Exception ex)
                        {
                            // Check if error is about DELIMITER (MySqlConnector limitation)
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

            // Remove SQL comments
            sqlContent = RemoveComments(sqlContent);

            // Normalize line endings
            sqlContent = Regex.Replace(sqlContent, @"\r\n|\r|\n", "\n");

            // Handle DELIMITER and multi-statement blocks
            var delimiter = ";";
            var currentQuery = new StringBuilder();
            var lines = sqlContent.Split('\n');
            var inStoredObject = false;
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                // Check for DELIMITER declaration
                if (trimmedLine.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the new delimiter
                    var parts = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        delimiter = parts[1];
                    }
                    continue; // Skip DELIMITER statements
                }

                // Check if we're starting a stored object definition
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
                    // DROP statements are typically single line
                    var dropQuery = trimmedLine;
                    if (dropQuery.EndsWith(delimiter))
                    {
                        dropQuery = dropQuery.Substring(0, dropQuery.Length - delimiter.Length);
                    }
                    if (!string.IsNullOrWhiteSpace(dropQuery))
                    {
                        queries.Add(dropQuery.Trim());
                    }
                    delimiter = ";"; // Reset
                    continue;
                }

                // Add line to current query
                if (currentQuery.Length > 0)
                    currentQuery.Append("\n");
                currentQuery.Append(line);

                // Check if we've reached the end of a query
                bool isEndOfQuery = false;

                if (inStoredObject)
                {
                    // For stored objects, look for END followed by the delimiter
                    if (upperLine.StartsWith("END") && trimmedLine.EndsWith(delimiter))
                    {
                        isEndOfQuery = true;
                        inStoredObject = false;
                    }
                }
                else
                {
                    // For regular queries, look for delimiter
                    if (trimmedLine.EndsWith(delimiter))
                    {
                        isEndOfQuery = true;
                    }
                }

                if (isEndOfQuery)
                {
                    var query = currentQuery.ToString().Trim();

                    // Remove trailing delimiter
                    if (query.EndsWith(delimiter))
                    {
                        query = query.Substring(0, query.Length - delimiter.Length);
                    }

                    query = query.Trim();

                    // Add query if not empty
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        queries.Add(query);
                    }

                    currentQuery.Clear();
                    delimiter = ";"; // Reset to default
                }
            }

            // Handle any remaining query
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
            // Remove single-line comments (-- comment)
            sql = Regex.Replace(sql, @"--.*?(?=\n|$)", "", RegexOptions.Multiline);

            // Remove multi-line comments (/* comment */)
            sql = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);

            // Remove inline comments (# comment)
            sql = Regex.Replace(sql, @"#.*?(?=\n|$)", "", RegexOptions.Multiline);

            return sql;
        }

        public async Task<bool> TestConnectionAsync(int databaseNumber)
        {
            try
            {
                var connectionString = databaseNumber == 1 ? _connectionString1 : _connectionString2;
                using (var connection = await OpenConnectionAsync(connectionString))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class ExecutionResult
    {
        public bool HasError { get; set; }
        public string Message { get; set; } = "";
        public int TotalQueries { get; set; }
        public DatabaseExecutionResult? Database1Results { get; set; }
        public DatabaseExecutionResult? Database2Results { get; set; }
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
