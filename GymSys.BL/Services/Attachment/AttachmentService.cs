using GymSys.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; //5 MB
        private readonly string[] _allowedExtensions = { ".png", ".jpeg", "jpg" };
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public Result Delete(string folderName, string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, folderName, fileName);
            try
            {
                if (!File.Exists(filePath)) return Result.NotFound("File not found!");
                File.Delete(filePath);
                return Result.OK();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete file {fileName}");
                return Result.Fail("Failed to delete file!");
            }
        }

        public Result<(Stream stream, string contentType)?> GetFile(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return Result<(Stream stream, string contentType)?>.NotFound("File name or folder name is missing!");

            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, folderName, fileName);
            if (!File.Exists(filePath)) return Result<(Stream stream, string contentType)?>.NotFound("File not found!");

            var openStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(filePath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return Result<(Stream stream, string contentType)?>.OK((openStream, contentType));
        }

        public async Task<Result<string?>> UploadAsync(Stream fileStream, string folderName, string fileName, CancellationToken ct = default)
        {
            if (fileStream == null) return Result<string?>.Fail("No file stream!");
            if (!fileStream.CanRead) return Result<string?>.Fail("Cannot read from this file stream!");
            if (fileStream.Length == 0) return Result<string?>.Fail("Empty file stream!");

            if (fileStream.Length > _maxFileSize)
            {
                _logger.LogError($"File rejected, File is too large - {fileStream.Length} bytes!");
                return Result<string?>.Validation("Maximum size is 5 MB!");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"File rejected, Extension {extension} not allowed!");
                return Result<string?>.Validation("File extension not allowed!");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            try
            {
                using var writeFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(writeFileStream, ct);
                return Result<string?>.OK(storedFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload file {fileName}");
                return Result<string?>.Fail("Failed to upload file!");
            }
        }
    }
}
