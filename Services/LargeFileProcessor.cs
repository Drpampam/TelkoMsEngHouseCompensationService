using System.Text.RegularExpressions;
using Npgsql;
using TLRProcessor.Models;
using TLRProcessor.Repositories;

namespace TLRProcessor.Services;

public class LargeFileProcessor
{
    private readonly ISmsTlrRepository _repo;
    private const int BatchSize = 1000;
    private readonly IConfiguration _config;


    public LargeFileProcessor(ISmsTlrRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task ProcessAsync(string filePath)
    {
        const int expectedFieldCount = 55; // update if fields increase
        var batch = new List<SmsTlrRecord>();


        var fileName = Path.GetFileName(filePath);


        using var reader = new StreamReader(filePath);
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            var parts = line.Split('|');
            if (parts.Length < expectedFieldCount)
                continue; // or log invalid line

            SmsTlrRecord record = new SmsTlrRecord
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
                ContentType = parts.Length > 56 ? parts[56] : null, // just in case line has extra fields
                DestinationDomain = parts.Length > 57 ? parts[57] : null,
                MessageState = parts.Length > 58 ? ParseInt(parts[58]) : 0
            };

            batch.Add(record);
            if (batch.Count >= BatchSize)
            {
                await _repo.BulkInsertAsyncV2(batch);
                batch.Clear();
            }
        }

        if (batch.Any())
            await _repo.BulkInsertAsyncV2(batch);

        // Helper parsing functions
        DateTime? ParseDateTime(string input) =>
            DateTime.TryParse(input, out var dt) && dt >= System.Data.SqlTypes.SqlDateTime.MinValue.Value
                ? dt
                : (DateTime?)null;

        int ParseInt(string input) =>
            int.TryParse(input, out var val) ? val : 0;

        decimal ParseDecimal(string input) =>
            decimal.TryParse(input, out var val) ? val : 0m;

        long ParseLong(string input) =>
            long.TryParse(input, out var val) ? val : 0L;
    }

    string ExtractUniqueNumberFromFileName(string fileName)
    {
        var match = Regex.Match(fileName, @"_(\d+)\.csv$");
        return match.Success ? match.Groups[1].Value : throw new Exception("Unique number not found in filename.");
    }

}