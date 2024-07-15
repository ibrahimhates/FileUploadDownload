using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FileUpload.Service;

public class FileService : IFileService
{
    private const string _fileDirectory = "Files";
    private readonly LocalDb _localDb;

    public FileService(LocalDb localDb)
    {
        _localDb=localDb;
    }

    public async Task<FileStreamResult> DownloadFileAsync(Guid id)
    {
        var model = await _localDb.ReadAsync(id);

        if (model == null)
            return null;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileDirectory, model.FilePath);

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(model.FilePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return new FileStreamResult(fileStream, contentType)
        {
            FileDownloadName = model.FileAlias
        };
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileName = file.FileName;
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

        var folder = Path.Combine(Directory.GetCurrentDirectory(), _fileDirectory);

        try
        {
            if (!Directory.Exists(_fileDirectory))
            {
                Directory.CreateDirectory(_fileDirectory);
            }

            var path = Path.Combine(folder, newFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var model = new LocalDbModel
            {
                Id = Guid.NewGuid(),
                FilePath = newFileName,
                FileAlias = fileName
            };

            await _localDb.WriteAsync(model);

            return "Dosya basariyla kaydedildi";
        }
        catch (Exception err)
        {
            return $"Dosya kayit islemide hata olustu. Error:{err.Message}";
        }
    }
}
