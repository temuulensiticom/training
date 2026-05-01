namespace DbSyncTool.Models
{
    public class DbConnectionConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; }
        public string Username { get; set; } = "root";
        public string Password { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;

        public string ConnectionString =>
            $"Server={Host};Port={Port};Database={Database};Uid={Username};Pwd={Password};" +
            $"AllowUserVariables=True;Allow Zero Datetime=True;" +
            $"ConnectionTimeout=30;DefaultCommandTimeout=120;";
    }

    public class ExecutionResult
    {
        public string DatabaseName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatementsExecuted { get; set; }
        public int StatementsSkipped { get; set; }
        public List<StatementResult> StatementResults { get; set; } = new();
        public TimeSpan Duration { get; set; }
    }

    public class StatementResult
    {
        public int Index { get; set; }
        public string StatementPreview { get; set; } = string.Empty;
        public string StatementType { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsIgnored { get; set; }
        public string? ErrorMessage { get; set; }
        public int RowsAffected { get; set; }
    }
}
