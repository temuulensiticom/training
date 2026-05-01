using System.Text;

namespace form.Services
{
    public class FileUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadFolder;

        public FileUploadService(IConfiguration configuration, ILogger<FileUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Create uploads folder if it doesn't exist
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public async Task<FileUploadResult> UploadAndReadFileAsync(IFormFile file)
        {
            var result = new FileUploadResult();

            try
            {
                // Validate file
                var validation = ValidateFile(file);
                if (!validation.IsValid)
                {
                    result.HasError = true;
                    result.Message = validation.Message;
                    return result;
                }

                // Check for duplicate file
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_uploadFolder, fileName);
                var fileIndex = 1;

                while (File.Exists(filePath))
                {
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    fileName = $"{nameWithoutExt}_{fileIndex}{extension}";
                    filePath = Path.Combine(_uploadFolder, fileName);
                    fileIndex++;
                    result.IsDuplicate = true;
                    result.DuplicateMessage = $"File name already exists. Saved as: {fileName}";
                }

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded successfully: {fileName}");

                // Read file content
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    result.Content = await reader.ReadToEndAsync();
                }

                result.FileName = fileName;
                result.FilePath = filePath;
                result.HasError = false;
                result.Message = "File uploaded and read successfully!";
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Message = $"Error processing file: {ex.Message}";
                _logger.LogError(ex, "Error in file upload");
            }

            return result;
        }

        private ValidationResult ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ValidationResult { IsValid = false, Message = "No file selected." };
            }

            var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5242880);
            if (file.Length > maxFileSize)
            {
                return new ValidationResult { IsValid = false, Message = $"File size exceeds limit of {maxFileSize / (1024 * 1024)}MB." };
            }

            var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() ?? new[] { ".sql", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult { IsValid = false, Message = $"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}" };
            }

            return new ValidationResult { IsValid = true, Message = "File is valid." };
        }

        public void DeleteFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted: {fileName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {fileName}");
            }
        }
    }

    public class FileUploadResult
    {
        public bool HasError { get; set; }
        public string Message { get; set; } = "";
        public string Content { get; set; } = "";
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public bool IsDuplicate { get; set; }
        public string DuplicateMessage { get; set; } = "";
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";
    }
}
