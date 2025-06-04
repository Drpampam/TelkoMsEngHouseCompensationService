using TLRProcessor.Common;

namespace TLRProcessor.Models;

public class SmsTlrRecord2 
{
    public string From { get; set; }
    public string To { get; set; }
    public string Message { get; set; }
    public DateTime? Timestamp { get; set; }
    public string MessageType { get; set; }
    public string Status { get; set; }
    public string ErrorCode { get; set; }
    public string MessageId { get; set; }
    public string AspName { get; set; }
    public string Application { get; set; }
}

public class SmsTlrRecord : BaseEntity
{
    public string FileName { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Message { get; set; }
    public DateTime? Timestamp { get; set; }
    public string MessageType { get; set; }
    public string Status { get; set; }
    public string ErrorCode { get; set; }
    public string AspName { get; set; }
    public string Application { get; set; }
    public string ScId { get; set; }
    public string ScConnection { get; set; }
    public string MessageId { get; set; }
    public string ChargeMsisdn { get; set; }
    public decimal ChargeAmount { get; set; }
    public int ChargeMethod { get; set; }
    public string ChargeSequenceId { get; set; }
    public long MinCredit { get; set; }
    public string ServiceName { get; set; }
    public string Reason { get; set; }
    public DateTime? Timestamp1 { get; set; }
    public DateTime? Timestamp2 { get; set; }
    public DateTime? Timestamp3 { get; set; }
    public string SiProtocol { get; set; }
    public string Ston { get; set; }
    public string Snpi { get; set; }
    public string Dton { get; set; }
    public string Dnpi { get; set; }
    public string DataCoding { get; set; }
    public string EsmClass { get; set; }
    public int TotalMessageNb { get; set; }
    public string VlrId { get; set; }
    public string SiMessageId { get; set; }
    public string AdStatus { get; set; }
    public string CorrelationId { get; set; }
    public string MtFdaReturned { get; set; }
    public string APartyBillingType { get; set; }
    public string BPartyBillingType { get; set; }
    public string DestinationImsi { get; set; }
    public string SourceImsi { get; set; }
    public string DestinationMsc { get; set; }
    public string SourceMsc { get; set; }
    public int CurrentNode { get; set; }
    public int FinalNode { get; set; }
    public string UniqueId { get; set; }
    public int ConnectionType { get; set; }
    public int Rtt { get; set; }
    public string PduType { get; set; }
    public string SourceSpName { get; set; }
    public string DestinationSpName { get; set; }
    public string Description { get; set; }
    public string OriginatingIp { get; set; }
    public string TerminatingIp { get; set; }
    public string OriginatingInterface { get; set; }
    public string OriginatingDomain { get; set; }
    public string DestinationInterface { get; set; }
    public string ContentType { get; set; }
    public string DestinationDomain { get; set; }
    public int MessageState { get; set; }
}
