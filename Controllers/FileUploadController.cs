using Microsoft.AspNetCore.Mvc;
using TLRProcessor.DTOs;
using TLRProcessor.Responses;
using TLRProcessor.Services;

namespace TLRProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly ILargeFileProcessor _fileProcessor;

    public FileUploadController(ILargeFileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
    }

    [RequestSizeLimit(1_000_000_000)] // 1 GB
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {

        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        var response = await _fileProcessor.UploadFileAsync(file);
        if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
        else if (response.ResponseCode == ResponseCodes.UNSUPPORTED_MEDIA_TYPE) { return StatusCode(207, response); }
        else if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
        return StatusCode(500, response);
    }

    [HttpPost("reporting")]
    public async Task<IActionResult> GetAll([FromBody] GetReporting filter)
    {
        var response = await _fileProcessor.GetAllContacts(filter);
        if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
        else if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
        return StatusCode(500, response);
    }
}