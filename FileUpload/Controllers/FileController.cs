using FileUpload.Service;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly LocalDb _localDb;
    public FileController(IFileService fileService, LocalDb localDb)
    {
        _fileService = fileService;
        _localDb=localDb;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        var response = await _fileService.UploadFileAsync(file);

        return Ok(response);
    }

    [HttpGet("download/{id:guid}")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var file = await _fileService.DownloadFileAsync(id);

        if (file == null)
        {
            return NotFound("Dosya bulunamadi");
        }

        return file;
    }

    [HttpGet]
    public async Task<IActionResult> AllFile()
    {
        var allFile = await _localDb.ReadAllAsync();

        var dto = allFile.Select(x => new LocalDbModelDto
        {
            Id = x.Id,
            FileAlias = x.FileAlias
        });

        return Ok(dto);
    }
}

class LocalDbModelDto
{
    public Guid Id { get; set; }
    public string FileAlias { get; set; }
}