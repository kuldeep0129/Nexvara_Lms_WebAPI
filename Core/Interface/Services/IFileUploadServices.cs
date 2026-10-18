using Core.Dtos.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface.Services
{
    public interface IFileUploadServices
    {
        Task<string> UploadAsync(string folderName, IFormFile formFile);
        Task<(Stream Stream, string contentType, string FileName)?> GetFileAsync(string relativePath);
        Task<bool> DeleteFileAsync(string relativePath);

        Task<FileUploadResultDto> UploadCloubinaryAsync(  IFormFile file,  string folder, CancellationToken cancellationToken = default);

        Task DeleteCloubinaryAsync( string publicId, CancellationToken cancellationToken = default);
    }
}
