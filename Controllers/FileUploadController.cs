using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TLRProcessor.Jobs;

namespace TLRProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IBackgroundJobClient _backgroundJobs;

    public FileUploadController(IWebHostEnvironment env, IBackgroundJobClient backgroundJobs)
    {
        _env = env;
        _backgroundJobs = backgroundJobs;
    }

    [RequestSizeLimit(1_000_000_000)] // 1 GB
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.Combine(_env.ContentRootPath, "Uploads", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _backgroundJobs.Enqueue<SmsTlrJob>(job => job.RunAsync(filePath));
        return Ok(new { message = "File uploaded and processing job started." });
    }
}