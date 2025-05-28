using System.Text.Json;
using System.Text.RegularExpressions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TLRProcessor.DTOs;
using TLRProcessor.Jobs;
using TLRProcessor.Models;
using TLRProcessor.Pagination;
using TLRProcessor.Repositories;
using TLRProcessor.Responses;
using static TLRProcessor.DTOs.GetReporting;

namespace TLRProcessor.Services;

public class LargeFileProcessor : ILargeFileProcessor
{
    private readonly ISmsTlrRepository _repo;
    private readonly IWebHostEnvironment _env;
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IConfiguration _config;
    private readonly ILogger<LargeFileProcessor> _logger;
    private const int BatchSize = 1000;

    public LargeFileProcessor(
        ISmsTlrRepository repo,
        IConfiguration config,
        IWebHostEnvironment env,
        IBackgroundJobClient backgroundJobs,
        ILogger<LargeFileProcessor> logger)
    {
        _repo = repo;
        _config = config;
        _env = env;
        _backgroundJobs = backgroundJobs;
        _logger = logger;
    }

    public async Task<BaseResponse<string>> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("No file uploaded.");
            return new BaseResponse<string>("No file uploaded.", ResponseCodes.UNSUPPORTED_MEDIA_TYPE);
        }

        var fileName = Path.GetFileName(file.FileName);
        var uploadFolder = Path.Combine(_env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(uploadFolder);
        var savedFilePath = Path.Combine(uploadFolder, fileName);

        try
        {
            var connectionString = _config.GetConnectionString("DataConnectionStrings");

            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            var existsCmd = new NpgsqlCommand(@"
                SELECT 1 
                FROM ""SmsTlrRecords"" 
                WHERE ""FileName"" = @FileName 
                LIMIT 1", conn);
            existsCmd.Parameters.AddWithValue("@FileName", fileName);

            var exists = await existsCmd.ExecuteScalarAsync();
            if (exists != null)
            {
                _logger.LogWarning("Duplicate file upload attempt: {FileName}", fileName);
                return new BaseResponse<string>($"File '{fileName}' has already been processed.", ResponseCodes.DUPLICATE_RESOURCE);
            }

            await using (var stream = new FileStream(savedFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _backgroundJobs.Enqueue<SmsTlrJob>(job => job.RunAsync(savedFilePath));
            _logger.LogInformation("File uploaded and job enqueued: {FileName}", fileName);

            return new BaseResponse<string>("File uploaded and processing job started.", ResponseCodes.SUCCESS);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload or enqueue processing for file {FileName}", fileName);
            return new BaseResponse<string>(ex.Message, ResponseCodes.SERVER_ERROR);
        }
    }

    public async Task ProcessAsync(string filePath)
    {
        const int expectedFieldCount = 55;
        var batch = new List<SmsTlrRecord>();
        var fileName = Path.GetFileName(filePath);

        try
        {
            using var reader = new StreamReader(filePath);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split('|');
                if (parts.Length < expectedFieldCount)
                {
                    _logger.LogWarning("Skipping malformed line in file {FileName}: {Line}", fileName, line);
                    continue;
                }

                var record = CreateRecord(parts, fileName);
                batch.Add(record);

                if (batch.Count >= BatchSize)
                {
                    await _repo.BulkInsertAsyncV2(batch);
                    _logger.LogInformation("Inserted batch of {Count} records from {FileName}", batch.Count, fileName);
                    batch.Clear();
                }
            }

            if (batch.Any())
            {
                await _repo.BulkInsertAsyncV2(batch);
                _logger.LogInformation("Inserted final batch of {Count} records from {FileName}", batch.Count, fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FilePath}", filePath);
        }
    }
    public async Task<BaseResponse<PaginationResult<SmsTlrRecord>>> GetAllContacts(GetReporting? filter = null)
    {
        try
        {
            filter ??= new GetReporting();

            var logRequest = JsonSerializer.Serialize(filter);
            _logger.LogInformation("Fetching SMS TLR records with filter: {Filter}", logRequest);

            var dateFilter = new FilterDateConvert();
            filter.MapToFilterDateConvert(dateFilter);

            var (records, totalCount) = await _repo.GetSmsTlrRecordsAsync(filter);

            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

            var response = new PaginationResult<SmsTlrRecord>(
                records,
                filter.PageNumber,
                filter.PageSize,
                totalCount,
                totalPages);

            return new BaseResponse<PaginationResult<SmsTlrRecord>>(
                $"Retrieved page {filter.PageNumber} of requests successfully",
                response,
                ResponseCodes.SUCCESS);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching file details.");
            return new BaseResponse<PaginationResult<SmsTlrRecord>>
            {
                Message = "An error occurred while fetching file details.",
                ResponseCode = ResponseCodes.FAILURE
            };
        }
    }

    private SmsTlrRecord CreateRecord(string[] parts, string fileName)
    {
        return new SmsTlrRecord
        {
            FileName = fileName,
            From = parts[1],
            To = parts[2],
            Message = parts[3],
            Timestamp = ParseDateTime(parts[4]),
            MessageType = parts[5],
            Status = parts[6],
            ErrorCode = parts[7],
            AspName = parts[8],
            Application = parts[9],
            ScId = parts[10],
            ScConnection = parts[11],
            MessageId = parts[12],
            ChargeMsisdn = parts[13],
            ChargeAmount = ParseDecimal(parts[14]),
            ChargeMethod = ParseInt(parts[15]),
            ChargeSequenceId = parts[16],
            MinCredit = ParseLong(parts[17]),
            ServiceName = parts[18],
            Reason = parts[19],
            Timestamp1 = ParseDateTime(parts[20]),
            Timestamp2 = ParseDateTime(parts[21]),
            Timestamp3 = ParseDateTime(parts[22]),
            SiProtocol = parts[23],
            Ston = parts[24],
            Snpi = parts[25],
            Dton = parts[26],
            Dnpi = parts[27],
            DataCoding = parts[28],
            EsmClass = parts[29],
            TotalMessageNb = ParseInt(parts[30]),
            VlrId = parts[31],
            SiMessageId = parts[32],
            AdStatus = parts[33],
            CorrelationId = parts[34],
            MtFdaReturned = parts[35],
            APartyBillingType = parts[36],
            BPartyBillingType = parts[37],
            DestinationImsi = parts[38],
            SourceImsi = parts[39],
            DestinationMsc = parts[40],
            SourceMsc = parts[41],
            CurrentNode = ParseInt(parts[42]),
            FinalNode = ParseInt(parts[43]),
            UniqueId = parts[44],
            ConnectionType = ParseInt(parts[45]),
            Rtt = ParseInt(parts[46]),
            PduType = parts[47],
            SourceSpName = parts[48],
            DestinationSpName = parts[49],
            Description = parts[50],
            OriginatingIp = parts[51],
            TerminatingIp = parts[52],
            OriginatingInterface = parts[53],
            OriginatingDomain = parts[54],
            DestinationInterface = parts[55],
            ContentType = parts.Length > 56 ? parts[56] : null,
            DestinationDomain = parts.Length > 57 ? parts[57] : null,
            MessageState = parts.Length > 58 ? ParseInt(parts[58]) : 0
        };
    }

    private DateTime? ParseDateTime(string input) =>
        DateTime.TryParse(input, out var dt) && dt >= System.Data.SqlTypes.SqlDateTime.MinValue.Value ? dt : null;

    private int ParseInt(string input) =>
        int.TryParse(input, out var val) ? val : 0;

    private decimal ParseDecimal(string input) =>
        decimal.TryParse(input, out var val) ? val : 0m;

    private long ParseLong(string input) =>
        long.TryParse(input, out var val) ? val : 0L;

    private string ExtractUniqueNumberFromFileName(string fileName)
    {
        var match = Regex.Match(fileName, @"_(\d+)\.csv$");
        return match.Success ? match.Groups[1].Value : throw new Exception("Unique number not found in filename.");
    }
}
