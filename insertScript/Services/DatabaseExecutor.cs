using DbSyncTool.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DbSyncTool.Services
{
    public class DatabaseExecutor
    {
        private readonly DbConnectionConfig _config;

        // MySQL error codes we classify as "duplicate / already exists"
        private static readonly HashSet<int> _duplicateErrorCodes = new()
        {
            1007, // Can't create database – already exists
            1050, // Table already exists
            1061, // Duplicate key name (index)
            1062, // Duplicate entry (unique constraint data)
            1304, // Procedure already exists
            1305, // Function already exists  (also FUNCTION does not exist on DROP – handled below)
            1359, // Trigger already exists
            1537, // Event already exists
            1840, // @@GLOBAL.GTID_PURGED can only be set when @@GLOBAL.GTID_EXECUTED is empty (import edge case)
        };

        // Codes that are safe to ignore during batch import
        private static readonly HashSet<int> _ignorableErrorCodes = new()
        {
            1008, // Can't drop database – doesn't exist (DROP IF NOT EXISTS workaround)
            1091, // Can't DROP – check that column/key exists
            1146, // Table doesn't exist (harmless DROP TABLE IF NOT EXISTS)
        };

        public DatabaseExecutor(DbConnectionConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Test the connection to the database.
        /// </summary>
        public async Task<(bool ok, string message)> TestConnectionAsync()
        {
            try
            {
                await using var conn = new MySqlConnection(_config.ConnectionString);
                await conn.OpenAsync();
                return (true, $"Connected to {_config.Name} (MySQL {conn.ServerVersion})");
            }
            catch (Exception ex)
            {
                return (false, $"Connection failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute a list of parsed SQL statements against this database.
        /// </summary>
        public async Task<ExecutionResult> ExecuteStatementsAsync(
            List<ParsedStatement> statements,
            IProgress<(int current, int total, string message)>? progress = null,
            CancellationToken ct = default)
        {
            var result = new ExecutionResult { DatabaseName = _config.Name };
            var sw = Stopwatch.StartNew();

            try
            {
                await using var conn = new MySqlConnection(_config.ConnectionString);
                await conn.OpenAsync(ct);

                // Suppress safe-mode restrictions common during full dumps
                await ExecuteRawAsync(conn, "SET foreign_key_checks = 0;", ct);
                await ExecuteRawAsync(conn, "SET sql_mode = '';", ct);
                await ExecuteRawAsync(conn, "SET unique_checks = 0;", ct);

                for (int i = 0; i < statements.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var stmt = statements[i];
                    progress?.Report((i + 1, statements.Count,
                        $"[{_config.Name}] ({i + 1}/{statements.Count}) {stmt.Preview}"));

                    var sr = new StatementResult
                    {
                        Index = i + 1,
                        StatementPreview = stmt.Preview,
                        StatementType = stmt.Type
                    };

                    try
                    {
                        await using var cmd = new MySqlCommand(stmt.Text, conn)
                        {
                            CommandTimeout = 120
                        };
                        int rows = await cmd.ExecuteNonQueryAsync(ct);
                        sr.Success = true;
                        sr.RowsAffected = rows < 0 ? 0 : rows;
                        result.StatementsExecuted++;
                    }
                    catch (MySqlException mex)
                    {
                        sr.Success = false;
                        sr.ErrorMessage = $"[MySQL {mex.Number}] {mex.Message}";

                        if (_duplicateErrorCodes.Contains(mex.Number))
                        {
                            sr.IsDuplicate = true;
                            sr.ErrorMessage = $"⚠ DUPLICATE/EXISTS – {sr.ErrorMessage}";
                            result.StatementsSkipped++;
                        }
                        else if (_ignorableErrorCodes.Contains(mex.Number))
                        {
                            sr.IsIgnored = true;
                            sr.ErrorMessage = $"ℹ IGNORED (safe) – {sr.ErrorMessage}";
                            result.StatementsSkipped++;
                        }
                        else
                        {
                            result.StatementsExecuted++;
                        }
                    }
                    catch (Exception ex)
                    {
                        sr.Success = false;
                        sr.ErrorMessage = $"ERROR: {ex.Message}";
                        result.StatementsExecuted++;
                    }

                    result.StatementResults.Add(sr);
                }

                // Restore
                await ExecuteRawAsync(conn, "SET foreign_key_checks = 1;", ct);
                await ExecuteRawAsync(conn, "SET unique_checks = 1;", ct);

                result.Success = true;
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.ErrorMessage = "Execution was cancelled by user.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Fatal connection error: {ex.Message}";
            }

            sw.Stop();
            result.Duration = sw.Elapsed;
            return result;
        }

        private static async Task ExecuteRawAsync(MySqlConnection conn, string sql, CancellationToken ct)
        {
            try
            {
                await using var cmd = new MySqlCommand(sql, conn) { CommandTimeout = 10 };
                await cmd.ExecuteNonQueryAsync(ct);
            }
            catch { /* best-effort */ }
        }
    }
}
