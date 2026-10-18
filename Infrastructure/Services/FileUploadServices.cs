using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Configuration;
using Core.Dtos.Common;
using Core.Interface.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileUploadServices : IFileUploadServices
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _webRoot;
        private readonly FileExtensionContentTypeProvider _contentTypes = new();
        public FileUploadServices(IWebHostEnvironment environment, IOptions<CloudinarySettings> cloudinarySettings)
        {
            _webRoot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(_webRoot);
            var setting = cloudinarySettings.Value;
            var account = new Account
            (
                  setting.CloudName,
            setting.ApiKey,
            setting.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task DeleteCloubinaryAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return;
            }

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new Exception(
                    $"Cloudinary delete failed: {result.Error.Message}");
            }
        }

        public  Task<bool> DeleteFileAsync(string relativePath)
        {
            var fullPath = ResolvePath(relativePath);

            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        public  Task<(Stream Stream, string contentType, string FileName)?> GetFileAsync(string relativePath)
        {
          
        var fullPath = ResolvePath(relativePath);

        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(null);

        if (!_contentTypes.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        var result = (
            Stream: stream,
            ContentType: contentType,
            FileName: Path.GetFileName(fullPath));

        return Task.FromResult<(Stream Stream, string ContentType, string FileName)?>(result);
        }

        public async Task<string> UploadAsync(string folderName, IFormFile formFile)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Folder name is required.");

            if (formFile == null || formFile.Length == 0)
                throw new ArgumentException("Please select a valid file.");

            if (folderName is "." or ".." || folderName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("Invalid folder name.");
            }
            if (formFile.Length > 10 * 1024 * 1024)
                throw new ArgumentException("Maximum file size is 10 MB.");

            var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
              {
                  ".jpg", ".jpeg", ".png", ".webp", ".gif",
                  ".pdf", ".mp4"
              };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("This file type is not allowed.");

            var folderPath = Path.Combine(_webRoot, "uploads", folderName); Directory.CreateDirectory(folderPath);

            var storedName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folderPath, storedName);

            await using (var output = new FileStream( fullPath, FileMode.CreateNew,FileAccess.Write, FileShare.None,bufferSize: 81920, useAsync: true))
            {
                await formFile.CopyToAsync(output);
            }

            return $"/uploads/{folderName}/{storedName}";
        }

        public async Task<FileUploadResultDto> UploadCloubinaryAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required.");
            }

            // 5 MB maximum file size
            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new ArgumentException(
                    "File size cannot be greater than 5 MB.");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException(
                    "Only image files are allowed.");
            }

            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException(
                    "Cloudinary folder is required.");
            }

            var publicId = $"{folder.Trim('/')}/{Guid.NewGuid():N}";

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new Exception(
                    $"Cloudinary upload failed: {result.Error.Message}");
            }

            return new FileUploadResultDto
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty
            };
        }

        private string ResolvePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("File path is required.");

            const string prefix = "/uploads/";

            if (!relativePath.StartsWith(prefix, StringComparison.Ordinal))
                throw new ArgumentException("Invalid upload path.");

            var relative = relativePath[prefix.Length..];

            var root = Path.GetFullPath(
                Path.Combine(_webRoot, "uploads") + Path.DirectorySeparatorChar);

            var fullPath = Path.GetFullPath(
                Path.Combine(root, relative.Replace(
                    '/', Path.DirectorySeparatorChar)));

            if (!fullPath.StartsWith(root, StringComparison.Ordinal))
                throw new ArgumentException("Invalid file path.");

            return fullPath;
        }
    }
}
