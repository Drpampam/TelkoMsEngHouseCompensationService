using Dapper;
using Npgsql;
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

    public Task BulkInsertAsync(IEnumerable<SmsTlrRecord> records)
    {
        throw new NotImplementedException();
    }

    //public async Task BulkInsertAsync(IEnumerable<SmsTlrRecord> records)
    //{
    //    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    //    var sql = @"
    //        INSERT INTO SmsTlrRecords ([From], [To], Message, Timestamp, MessageType, Status, ErrorCode, MessageId, AspName, Application)
    //        VALUES (@From, @To, @Message, @Timestamp, @MessageType, @Status, @ErrorCode, @MessageId, @AspName, @Application);";
    //    await conn.ExecuteAsync(sql, records);
    //}

    //public async Task BulkInsertAsyncV3(IEnumerable<SmsTlrRecord> records)
    //{
    //    var dataTable = new DataTable();
    //    dataTable.Columns.Add("Id", typeof(string));
    //    dataTable.Columns.Add("From", typeof(string));
    //    dataTable.Columns.Add("To", typeof(string));
    //    dataTable.Columns.Add("Message", typeof(string));
    //    dataTable.Columns.Add("Timestamp", typeof(DateTime));
    //    dataTable.Columns.Add("MessageType", typeof(string));
    //    dataTable.Columns.Add("Status", typeof(string));
    //    dataTable.Columns.Add("ErrorCode", typeof(string));
    //    dataTable.Columns.Add("AspName", typeof(string));
    //    dataTable.Columns.Add("Application", typeof(string));
    //    dataTable.Columns.Add("ScId", typeof(string));
    //    dataTable.Columns.Add("ScConnection", typeof(string));
    //    dataTable.Columns.Add("MessageId", typeof(string));
    //    dataTable.Columns.Add("ChargeMsisdn", typeof(string));
    //    dataTable.Columns.Add("ChargeAmount", typeof(decimal));
    //    dataTable.Columns.Add("ChargeMethod", typeof(int));
    //    dataTable.Columns.Add("ChargeSequenceId", typeof(string));
    //    dataTable.Columns.Add("MinCredit", typeof(long));
    //    dataTable.Columns.Add("ServiceName", typeof(string));
    //    dataTable.Columns.Add("Reason", typeof(string));
    //    dataTable.Columns.Add("Timestamp1", typeof(DateTime));
    //    dataTable.Columns.Add("Timestamp2", typeof(DateTime));
    //    dataTable.Columns.Add("Timestamp3", typeof(DateTime));
    //    dataTable.Columns.Add("SiProtocol", typeof(string));
    //    dataTable.Columns.Add("Ston", typeof(string));
    //    dataTable.Columns.Add("Snpi", typeof(string));
    //    dataTable.Columns.Add("Dton", typeof(string));
    //    dataTable.Columns.Add("Dnpi", typeof(string));
    //    dataTable.Columns.Add("DataCoding", typeof(string));
    //    dataTable.Columns.Add("EsmClass", typeof(string));
    //    dataTable.Columns.Add("TotalMessageNb", typeof(int));
    //    dataTable.Columns.Add("VlrId", typeof(string));
    //    dataTable.Columns.Add("SiMessageId", typeof(string));
    //    dataTable.Columns.Add("AdStatus", typeof(string));
    //    dataTable.Columns.Add("CorrelationId", typeof(string));
    //    dataTable.Columns.Add("MtFdaReturned", typeof(string));
    //    dataTable.Columns.Add("APartyBillingType", typeof(string));
    //    dataTable.Columns.Add("BPartyBillingType", typeof(string));
    //    dataTable.Columns.Add("DestinationImsi", typeof(string));
    //    dataTable.Columns.Add("SourceImsi", typeof(string));
    //    dataTable.Columns.Add("DestinationMsc", typeof(string));
    //    dataTable.Columns.Add("SourceMsc", typeof(string));
    //    dataTable.Columns.Add("CurrentNode", typeof(int));
    //    dataTable.Columns.Add("FinalNode", typeof(int));
    //    dataTable.Columns.Add("UniqueId", typeof(string));
    //    dataTable.Columns.Add("ConnectionType", typeof(int));
    //    dataTable.Columns.Add("Rtt", typeof(int));
    //    dataTable.Columns.Add("PduType", typeof(string));
    //    dataTable.Columns.Add("SourceSpName", typeof(string));
    //    dataTable.Columns.Add("DestinationSpName", typeof(string));
    //    dataTable.Columns.Add("Description", typeof(string));
    //    dataTable.Columns.Add("OriginatingIp", typeof(string));
    //    dataTable.Columns.Add("TerminatingIp", typeof(string));
    //    dataTable.Columns.Add("OriginatingInterface", typeof(string));
    //    dataTable.Columns.Add("OriginatingDomain", typeof(string));
    //    dataTable.Columns.Add("DestinationInterface", typeof(string));
    //    dataTable.Columns.Add("ContentType", typeof(string));
    //    dataTable.Columns.Add("DestinationDomain", typeof(string));
    //    dataTable.Columns.Add("MessageState", typeof(int));

    //    foreach (var r in records)
    //    {
    //        dataTable.Rows.Add(
    //            r.Id ?? Guid.NewGuid().ToString(),
    //            r.From,
    //            r.To,
    //            r.Message,
    //            r.Timestamp ?? (object)DBNull.Value,
    //            r.MessageType,
    //            r.Status,
    //            r.ErrorCode,
    //            r.AspName,
    //            r.Application,
    //            r.ScId,
    //            r.ScConnection,
    //            r.MessageId,
    //            r.ChargeMsisdn,
    //            r.ChargeAmount,
    //            r.ChargeMethod,
    //            r.ChargeSequenceId,
    //            r.MinCredit,
    //            r.ServiceName,
    //            r.Reason,
    //            r.Timestamp1 ?? (object)DBNull.Value,
    //            r.Timestamp2 ?? (object)DBNull.Value,
    //            r.Timestamp3 ?? (object)DBNull.Value,
    //            r.SiProtocol,
    //            r.Ston,
    //            r.Snpi,
    //            r.Dton,
    //            r.Dnpi,
    //            r.DataCoding,
    //            r.EsmClass,
    //            r.TotalMessageNb,
    //            r.VlrId,
    //            r.SiMessageId,
    //            r.AdStatus,
    //            r.CorrelationId,
    //            r.MtFdaReturned,
    //            r.APartyBillingType,
    //            r.BPartyBillingType,
    //            r.DestinationImsi,
    //            r.SourceImsi,
    //            r.DestinationMsc,
    //            r.SourceMsc,
    //            r.CurrentNode,
    //            r.FinalNode,
    //            r.UniqueId,
    //            r.ConnectionType,
    //            r.Rtt,
    //            r.PduType,
    //            r.SourceSpName,
    //            r.DestinationSpName,
    //            r.Description,
    //            r.OriginatingIp,
    //            r.TerminatingIp,
    //            r.OriginatingInterface,
    //            r.OriginatingDomain,
    //            r.DestinationInterface,
    //            r.ContentType,
    //            r.DestinationDomain,
    //            r.MessageState
    //        );
    //    }

    //    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    //    await conn.OpenAsync();

    //    using var bulkCopy = new SqlBulkCopy(conn)
    //    {
    //        DestinationTableName = "SmsTlrRecords",
    //        BatchSize = 5000,
    //        BulkCopyTimeout = 600 // seconds, adjust as needed
    //    };

    //    // Map columns explicitly if your DataTable column names differ from table column names:
    //    foreach (DataColumn col in dataTable.Columns)
    //    {
    //        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
    //    }

    //    await bulkCopy.WriteToServerAsync(dataTable);
    //}

    public async Task BulkInsertAsyncV2(IEnumerable<SmsTlrRecord> records)
    {
        var connectionString = _config.GetConnectionString("DataConnectionStrings");

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        using var writer = conn.BeginBinaryImport(@"
        COPY ""SmsTlrRecords"" (
            ""Id"", ""From"", ""To"", ""Message"", ""Timestamp"", ""MessageType"", ""Status"",
            ""ErrorCode"", ""AspName"", ""Application"", ""ScId"", ""ScConnection"", ""MessageId"",
            ""ChargeMsisdn"", ""ChargeAmount"", ""ChargeMethod"", ""ChargeSequenceId"", ""MinCredit"",
            ""ServiceName"", ""Reason"", ""Timestamp1"", ""Timestamp2"", ""Timestamp3"", ""SiProtocol"",
            ""Ston"", ""Snpi"", ""Dton"", ""Dnpi"", ""DataCoding"", ""EsmClass"", ""TotalMessageNb"",
            ""VlrId"", ""SiMessageId"", ""AdStatus"", ""CorrelationId"", ""MtFdaReturned"",
            ""APartyBillingType"", ""BPartyBillingType"", ""DestinationImsi"", ""SourceImsi"",
            ""DestinationMsc"", ""SourceMsc"", ""CurrentNode"", ""FinalNode"", ""UniqueId"",
            ""ConnectionType"", ""Rtt"", ""PduType"", ""SourceSpName"", ""DestinationSpName"",
            ""Description"", ""OriginatingIp"", ""TerminatingIp"", ""OriginatingInterface"",
            ""OriginatingDomain"", ""DestinationInterface"", ""ContentType"", ""DestinationDomain"",
            ""MessageState""
        ) FROM STDIN (FORMAT BINARY)");

        foreach (var r in records)
        {
            await writer.StartRowAsync();
            writer.Write(r.Id ?? Guid.NewGuid().ToString(), NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.From, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.To, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Message, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Timestamp, NpgsqlTypes.NpgsqlDbType.Timestamp);
            writer.Write(r.MessageType, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Status, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ErrorCode, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.AspName, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Application, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ScId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ScConnection, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.MessageId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ChargeMsisdn, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ChargeAmount, NpgsqlTypes.NpgsqlDbType.Numeric);
            writer.Write(r.ChargeMethod, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.ChargeSequenceId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.MinCredit, NpgsqlTypes.NpgsqlDbType.Bigint);
            writer.Write(r.ServiceName, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Reason, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Timestamp1, NpgsqlTypes.NpgsqlDbType.Timestamp);
            writer.Write(r.Timestamp2, NpgsqlTypes.NpgsqlDbType.Timestamp);
            writer.Write(r.Timestamp3, NpgsqlTypes.NpgsqlDbType.Timestamp);
            writer.Write(r.SiProtocol, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Ston, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Snpi, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Dton, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Dnpi, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DataCoding, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.EsmClass, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.TotalMessageNb, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.VlrId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.SiMessageId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.AdStatus, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.CorrelationId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.MtFdaReturned, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.APartyBillingType, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.BPartyBillingType, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DestinationImsi, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.SourceImsi, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DestinationMsc, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.SourceMsc, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.CurrentNode, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.FinalNode, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.UniqueId, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ConnectionType, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.Rtt, NpgsqlTypes.NpgsqlDbType.Integer);
            writer.Write(r.PduType, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.SourceSpName, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DestinationSpName, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.Description, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.OriginatingIp, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.TerminatingIp, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.OriginatingInterface, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.OriginatingDomain, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DestinationInterface, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.ContentType, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.DestinationDomain, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(r.MessageState, NpgsqlTypes.NpgsqlDbType.Integer);
        }

        await writer.CompleteAsync();
    }

    public Task BulkInsertAsyncV3(IEnumerable<SmsTlrRecord> records)
    {
        throw new NotImplementedException();
    }


    //public async Task BulkInsertAsyncV4(IEnumerable<SmsTlrRecord> records)
    //{
    //    var sql = @"
    //    INSERT INTO SmsTlrRecords
    //    ([From], [To], Message, Timestamp, MessageType, Status, ErrorCode, AspName, Application, ScId, ScConnection, MessageId,
    //     ChargeMsisdn, ChargeAmount, ChargeMethod, ChargeSequenceId, MinCredit, ServiceName, Reason, Timestamp1, Timestamp2, Timestamp3,
    //     SiProtocol, Ston, Snpi, Dton, Dnpi, DataCoding, EsmClass, TotalMessageNb, VlrId, SiMessageId, AdStatus, CorrelationId, MtFdaReturned,
    //     APartyBillingType, BPartyBillingType, DestinationImsi, SourceImsi, DestinationMsc, SourceMsc, CurrentNode, FinalNode, UniqueId,
    //     ConnectionType, Rtt, PduType, SourceSpName, DestinationSpName, Description, OriginatingIp, TerminatingIp, OriginatingInterface,
    //     OriginatingDomain, DestinationInterface, ContentType, DestinationDomain, MessageState)
    //    VALUES
    //    (@From, @To, @Message, @Timestamp, @MessageType, @Status, @ErrorCode, @AspName, @Application, @ScId, @ScConnection, @MessageId,
    //     @ChargeMsisdn, @ChargeAmount, @ChargeMethod, @ChargeSequenceId, @MinCredit, @ServiceName, @Reason, @Timestamp1, @Timestamp2, @Timestamp3,
    //     @SiProtocol, @Ston, @Snpi, @Dton, @Dnpi, @DataCoding, @EsmClass, @TotalMessageNb, @VlrId, @SiMessageId, @AdStatus, @CorrelationId, @MtFdaReturned,
    //     @APartyBillingType, @BPartyBillingType, @DestinationImsi, @SourceImsi, @DestinationMsc, @SourceMsc, @CurrentNode, @FinalNode, @UniqueId,
    //     @ConnectionType, @Rtt, @PduType, @SourceSpName, @DestinationSpName, @Description, @OriginatingIp, @TerminatingIp, @OriginatingInterface,
    //     @OriginatingDomain, @DestinationInterface, @ContentType, @DestinationDomain, @MessageState);";

    //    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    //    await conn.OpenAsync();
    //    await conn.ExecuteAsync(sql, records);
    //}


}