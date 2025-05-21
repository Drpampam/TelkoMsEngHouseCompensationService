using Dapper;
using System.Data;
using System.Data.SqlClient;
using TLRProcessor.Models;

namespace TLRProcessor.Repositories;

public class SmsTlrRepository : ISmsTlrRepository
{
    private readonly IConfiguration _config;

    public SmsTlrRepository(IConfiguration config)
    {
        _config = config;
    }

    public async Task BulkInsertAsync(IEnumerable<SmsTlrRecord> records)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var sql = @"
            INSERT INTO SmsTlrRecords ([From], [To], Message, Timestamp, MessageType, Status, ErrorCode, MessageId, AspName, Application)
            VALUES (@From, @To, @Message, @Timestamp, @MessageType, @Status, @ErrorCode, @MessageId, @AspName, @Application);";
        await conn.ExecuteAsync(sql, records);
    }

    public async Task BulkInsertAsyncV2(IEnumerable<SmsTlrRecord> records)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(string));
        dataTable.Columns.Add("From", typeof(string));
        dataTable.Columns.Add("To", typeof(string));
        dataTable.Columns.Add("Message", typeof(string));
        dataTable.Columns.Add("Timestamp", typeof(DateTime));
        dataTable.Columns.Add("MessageType", typeof(string));
        dataTable.Columns.Add("Status", typeof(string));
        dataTable.Columns.Add("ErrorCode", typeof(string));
        dataTable.Columns.Add("AspName", typeof(string));
        dataTable.Columns.Add("Application", typeof(string));
        dataTable.Columns.Add("ScId", typeof(string));
        dataTable.Columns.Add("ScConnection", typeof(string));
        dataTable.Columns.Add("MessageId", typeof(string));
        dataTable.Columns.Add("ChargeMsisdn", typeof(string));
        dataTable.Columns.Add("ChargeAmount", typeof(decimal));
        dataTable.Columns.Add("ChargeMethod", typeof(int));
        dataTable.Columns.Add("ChargeSequenceId", typeof(string));
        dataTable.Columns.Add("MinCredit", typeof(long));
        dataTable.Columns.Add("ServiceName", typeof(string));
        dataTable.Columns.Add("Reason", typeof(string));
        dataTable.Columns.Add("Timestamp1", typeof(DateTime));
        dataTable.Columns.Add("Timestamp2", typeof(DateTime));
        dataTable.Columns.Add("Timestamp3", typeof(DateTime));
        dataTable.Columns.Add("SiProtocol", typeof(string));
        dataTable.Columns.Add("Ston", typeof(string));
        dataTable.Columns.Add("Snpi", typeof(string));
        dataTable.Columns.Add("Dton", typeof(string));
        dataTable.Columns.Add("Dnpi", typeof(string));
        dataTable.Columns.Add("DataCoding", typeof(string));
        dataTable.Columns.Add("EsmClass", typeof(string));
        dataTable.Columns.Add("TotalMessageNb", typeof(int));
        dataTable.Columns.Add("VlrId", typeof(string));
        dataTable.Columns.Add("SiMessageId", typeof(string));
        dataTable.Columns.Add("AdStatus", typeof(string));
        dataTable.Columns.Add("CorrelationId", typeof(string));
        dataTable.Columns.Add("MtFdaReturned", typeof(string));
        dataTable.Columns.Add("APartyBillingType", typeof(string));
        dataTable.Columns.Add("BPartyBillingType", typeof(string));
        dataTable.Columns.Add("DestinationImsi", typeof(string));
        dataTable.Columns.Add("SourceImsi", typeof(string));
        dataTable.Columns.Add("DestinationMsc", typeof(string));
        dataTable.Columns.Add("SourceMsc", typeof(string));
        dataTable.Columns.Add("CurrentNode", typeof(int));
        dataTable.Columns.Add("FinalNode", typeof(int));
        dataTable.Columns.Add("UniqueId", typeof(string));
        dataTable.Columns.Add("ConnectionType", typeof(int));
        dataTable.Columns.Add("Rtt", typeof(int));
        dataTable.Columns.Add("PduType", typeof(string));
        dataTable.Columns.Add("SourceSpName", typeof(string));
        dataTable.Columns.Add("DestinationSpName", typeof(string));
        dataTable.Columns.Add("Description", typeof(string));
        dataTable.Columns.Add("OriginatingIp", typeof(string));
        dataTable.Columns.Add("TerminatingIp", typeof(string));
        dataTable.Columns.Add("OriginatingInterface", typeof(string));
        dataTable.Columns.Add("OriginatingDomain", typeof(string));
        dataTable.Columns.Add("DestinationInterface", typeof(string));
        dataTable.Columns.Add("ContentType", typeof(string));
        dataTable.Columns.Add("DestinationDomain", typeof(string));
        dataTable.Columns.Add("MessageState", typeof(int));

        foreach (var r in records)
        {
            dataTable.Rows.Add(
                r.Id ?? Guid.NewGuid().ToString(),
                r.From,
                r.To,
                r.Message,
                r.Timestamp ?? (object)DBNull.Value,
                r.MessageType,
                r.Status,
                r.ErrorCode,
                r.AspName,
                r.Application,
                r.ScId,
                r.ScConnection,
                r.MessageId,
                r.ChargeMsisdn,
                r.ChargeAmount,
                r.ChargeMethod,
                r.ChargeSequenceId,
                r.MinCredit,
                r.ServiceName,
                r.Reason,
                r.Timestamp1 ?? (object)DBNull.Value,
                r.Timestamp2 ?? (object)DBNull.Value,
                r.Timestamp3 ?? (object)DBNull.Value,
                r.SiProtocol,
                r.Ston,
                r.Snpi,
                r.Dton,
                r.Dnpi,
                r.DataCoding,
                r.EsmClass,
                r.TotalMessageNb,
                r.VlrId,
                r.SiMessageId,
                r.AdStatus,
                r.CorrelationId,
                r.MtFdaReturned,
                r.APartyBillingType,
                r.BPartyBillingType,
                r.DestinationImsi,
                r.SourceImsi,
                r.DestinationMsc,
                r.SourceMsc,
                r.CurrentNode,
                r.FinalNode,
                r.UniqueId,
                r.ConnectionType,
                r.Rtt,
                r.PduType,
                r.SourceSpName,
                r.DestinationSpName,
                r.Description,
                r.OriginatingIp,
                r.TerminatingIp,
                r.OriginatingInterface,
                r.OriginatingDomain,
                r.DestinationInterface,
                r.ContentType,
                r.DestinationDomain,
                r.MessageState
            );
        }

        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(conn)
        {
            DestinationTableName = "SmsTlrRecords",
            BatchSize = 5000,
            BulkCopyTimeout = 600 // seconds, adjust as needed
        };

        // Map columns explicitly if your DataTable column names differ from table column names:
        foreach (DataColumn col in dataTable.Columns)
        {
            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        }

        await bulkCopy.WriteToServerAsync(dataTable);
    }

    public async Task BulkInsertAsyncV3(IEnumerable<SmsTlrRecord> records)
    {
        var sql = @"
        INSERT INTO SmsTlrRecords
        ([From], [To], Message, Timestamp, MessageType, Status, ErrorCode, AspName, Application, ScId, ScConnection, MessageId,
         ChargeMsisdn, ChargeAmount, ChargeMethod, ChargeSequenceId, MinCredit, ServiceName, Reason, Timestamp1, Timestamp2, Timestamp3,
         SiProtocol, Ston, Snpi, Dton, Dnpi, DataCoding, EsmClass, TotalMessageNb, VlrId, SiMessageId, AdStatus, CorrelationId, MtFdaReturned,
         APartyBillingType, BPartyBillingType, DestinationImsi, SourceImsi, DestinationMsc, SourceMsc, CurrentNode, FinalNode, UniqueId,
         ConnectionType, Rtt, PduType, SourceSpName, DestinationSpName, Description, OriginatingIp, TerminatingIp, OriginatingInterface,
         OriginatingDomain, DestinationInterface, ContentType, DestinationDomain, MessageState)
        VALUES
        (@From, @To, @Message, @Timestamp, @MessageType, @Status, @ErrorCode, @AspName, @Application, @ScId, @ScConnection, @MessageId,
         @ChargeMsisdn, @ChargeAmount, @ChargeMethod, @ChargeSequenceId, @MinCredit, @ServiceName, @Reason, @Timestamp1, @Timestamp2, @Timestamp3,
         @SiProtocol, @Ston, @Snpi, @Dton, @Dnpi, @DataCoding, @EsmClass, @TotalMessageNb, @VlrId, @SiMessageId, @AdStatus, @CorrelationId, @MtFdaReturned,
         @APartyBillingType, @BPartyBillingType, @DestinationImsi, @SourceImsi, @DestinationMsc, @SourceMsc, @CurrentNode, @FinalNode, @UniqueId,
         @ConnectionType, @Rtt, @PduType, @SourceSpName, @DestinationSpName, @Description, @OriginatingIp, @TerminatingIp, @OriginatingInterface,
         @OriginatingDomain, @DestinationInterface, @ContentType, @DestinationDomain, @MessageState);";

        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();
        await conn.ExecuteAsync(sql, records);
    }


}