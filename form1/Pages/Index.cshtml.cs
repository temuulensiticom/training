using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using form.Services;

namespace form.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseService _databaseService;
        private readonly FileUploadService _fileUploadService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(DatabaseService databaseService, FileUploadService fileUploadService, ILogger<IndexModel> logger)
        {
            _databaseService = databaseService;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public List<DatabaseTargetStatus> ConfiguredDatabases { get; set; } = new();
        public List<DatabaseTargetStatus> CustomDatabases { get; set; } = new();
        public string Message { get; set; } = "";
        public bool HasError { get; set; }
        public bool IsDuplicate { get; set; }
        public string DuplicateMessage { get; set; } = "";
        public List<DatabaseExecutionResult> DatabaseResults { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadConfiguredDatabaseStatusesAsync();
        }

        public async Task OnPostAsync(
            IFormFile? fileUpload,
            string[] selectedConnectionNames,
            string[] customDisplayName,
            string[] customHost,
            int[] customPort,
            string[] customDatabase,
            string[] customUser,
            string[] customPassword)
        {
            await LoadConfiguredDatabaseStatusesAsync();

            var selectedTargets = ConfiguredDatabases
                .Where(status => selectedConnectionNames.Contains(status.Target.Key))
                .Select(status => status.Target)
                .ToList();

            var customTargets = BuildCustomTargets(customDisplayName, customHost, customPort, customDatabase, customUser, customPassword);
            foreach (var target in customTargets)
            {
                var connected = await _databaseService.TestConnectionAsync(target.ConnectionString);
                CustomDatabases.Add(new DatabaseTargetStatus { Target = target, Connected = connected });

                if (connected)
                {
                    selectedTargets.Add(target);
                }
            }

            if (selectedTargets.Count == 0)
            {
                HasError = true;
                Message = "Please select at least one connected database to execute queries on.";
                return;
            }

            if (fileUpload == null || fileUpload.Length == 0)
            {
                HasError = true;
                Message = "No file selected. Please choose a file to upload.";
                return;
            }

            try
            {
                var fileResult = await _fileUploadService.UploadAndReadFileAsync(fileUpload);

                if (fileResult.HasError)
                {
                    HasError = true;
                    Message = $"{fileResult.Message}";
                    return;
                }

                if (fileResult.IsDuplicate)
                {
                    IsDuplicate = true;
                    DuplicateMessage = fileResult.DuplicateMessage;
                }

                var executionResult = await _databaseService.ExecuteQueriesAsync(
                    fileResult.Content,
                    selectedTargets
                );

                HasError = executionResult.HasError;
                Message = $"File '{fileResult.FileName}' processed | Total Queries: {executionResult.TotalQueries}";
                DatabaseResults = executionResult.DatabaseResults;

                _logger.LogInformation($"File processed successfully: {fileResult.FileName}");
            }
            catch (Exception ex)
            {
                HasError = true;
                Message = $" Fatal error: {ex.Message}";
                _logger.LogError(ex, "Error processing file");
            }
        }

        private async Task LoadConfiguredDatabaseStatusesAsync()
        {
            ConfiguredDatabases.Clear();

            foreach (var target in _databaseService.GetConfiguredTargets())
            {
                ConfiguredDatabases.Add(new DatabaseTargetStatus
                {
                    Target = target,
                    Connected = await _databaseService.TestConnectionAsync(target.ConnectionString)
                });
            }
        }

        private List<DatabaseTarget> BuildCustomTargets(
            string[] names,
            string[] hosts,
            int[] ports,
            string[] databases,
            string[] users,
            string[] passwords)
        {
            var targets = new List<DatabaseTarget>();
            var count = new[] { names.Length, hosts.Length, ports.Length, databases.Length, users.Length, passwords.Length }.Max();

            for (var i = 0; i < count; i++)
            {
                var database = GetValue(databases, i);
                var user = GetValue(users, i);

                if (string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(user))
                {
                    continue;
                }

                targets.Add(_databaseService.CreateCustomTarget(
                    GetValue(names, i),
                    GetValue(hosts, i),
                    GetIntValue(ports, i, 3306),
                    database,
                    user,
                    GetValue(passwords, i)));
            }

            return targets;
        }

        private static string GetValue(string[] values, int index)
        {
            return index < values.Length ? values[index] : "";
        }

        private static int GetIntValue(int[] values, int index, int fallback)
        {
            return index < values.Length && values[index] > 0 ? values[index] : fallback;
        }
    }
}
