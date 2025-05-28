using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TLRProcessor.Jobs;

namespace TLRProcessor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IConfiguration _config;


    public FileUploadController(IWebHostEnvironment env, IBackgroundJobClient backgroundJobs, IConfiguration config)
    {
        _env = env;
        _backgroundJobs = backgroundJobs;
        _config = config;
    }

    [RequestSizeLimit(1_000_000_000)] // 1 GB
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.Combine(_env.ContentRootPath, "Uploads", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);


        // Check if file has already been processed
        var connectionString = _config.GetConnectionString("DataConnectionStrings");

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var fileName = Path.GetFileName(filePath);

        var existsCmd = new NpgsqlCommand(@"
    SELECT 1 
    FROM ""SmsTlrRecords"" 
    WHERE ""FileName"" = @FileName 
    LIMIT 1", conn);

        existsCmd.Parameters.AddWithValue("@FileName", fileName);

        var exists = await existsCmd.ExecuteScalarAsync();
        if (exists != null)
        {
            throw new InvalidOperationException($"File '{fileName}' has already been processed.");
        }



        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _backgroundJobs.Enqueue<SmsTlrJob>(job => job.RunAsync(filePath));
        return Ok(new { message = "File uploaded and processing job started." });
    }
}