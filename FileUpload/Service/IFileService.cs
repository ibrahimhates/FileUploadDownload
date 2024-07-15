using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace FileUpload.Service;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<FileStreamResult> DownloadFileAsync(Guid id);
}
