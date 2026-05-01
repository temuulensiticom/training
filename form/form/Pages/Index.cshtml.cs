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

        // Properties for display
        public bool Database1Connected { get; set; }
        public bool Database2Connected { get; set; }
        public string Message { get; set; } = "";
        public bool HasError { get; set; }
        public bool IsDuplicate { get; set; }
        public string DuplicateMessage { get; set; } = "";
        public DatabaseExecutionResult? Database1Results { get; set; }
        public DatabaseExecutionResult? Database2Results { get; set; }

        public async Task OnGetAsync()
        {
            // Check database connections on page load
            Database1Connected = await _databaseService.TestConnectionAsync(1);
            Database2Connected = await _databaseService.TestConnectionAsync(2);
        }

        public async Task OnPostAsync(IFormFile? fileUpload, bool executeOnDb1, bool executeOnDb2)
        {
            // Check database connections
            Database1Connected = await _databaseService.TestConnectionAsync(1);
            Database2Connected = await _databaseService.TestConnectionAsync(2);

            // Validate that at least one database is selected
            if (!executeOnDb1 && !executeOnDb2)
            {
                HasError = true;
                Message = "Please select at least one database to execute queries on.";
                return;
            }

            // Validate file upload
            if (fileUpload == null || fileUpload.Length == 0)
            {
                HasError = true;
                Message = "No file selected. Please choose a file to upload.";
                return;
            }

            try
            {
                // Upload and read file
                var fileResult = await _fileUploadService.UploadAndReadFileAsync(fileUpload);

                if (fileResult.HasError)
                {
                    HasError = true;
                    Message = $"{fileResult.Message}";
                    return;
                }

                // Check for duplicate
                if (fileResult.IsDuplicate)
                {
                    IsDuplicate = true;
                    DuplicateMessage = fileResult.DuplicateMessage;
                }

                // Execute queries
                var executionResult = await _databaseService.ExecuteQueriesAsync(
                    fileResult.Content,
                    executeOnDb1 && Database1Connected,
                    executeOnDb2 && Database2Connected
                );

                HasError = executionResult.HasError;
                Message = $" File '{fileResult.FileName}' processed | Total Queries: {executionResult.TotalQueries}";

                if (executionResult.Database1Results != null)
                {
                    Database1Results = executionResult.Database1Results;
                }

                if (executionResult.Database2Results != null)
                {
                    Database2Results = executionResult.Database2Results;
                }

                // Add warning for disconnected databases
                if (!Database1Connected && executeOnDb1)
                {
                    Message += " <br/>  Database 1 is not connected - queries were not executed on it.";
                }
                if (!Database2Connected && executeOnDb2)
                {
                    Message += " <br/>  Database 2 is not connected - queries were not executed on it.";
                }

                _logger.LogInformation($"File processed successfully: {fileResult.FileName}");
            }
            catch (Exception ex)
            {
                HasError = true;
                Message = $" Fatal error: {ex.Message}";
                _logger.LogError(ex, "Error processing file");
            }
        }
    }
}
