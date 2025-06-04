namespace Application.Features.Configurations;

public record ZethConfiguration
{
    public string? CustomerInfo { get; set; }
    public string? AccountSummary { get; set; }
    public string? AccountBalance { get; set; }
    public string? CustomerInfoNew { get; set; }

}

public class SftpConnectionDetails
{
    public string UserId { get; set; }

    public string Password { get; set; }

    public string Host { get; set; }

    public string PortNumber { get; set; }

    public string HostKeyFingerPrint { get; set; }

    public string ArchivePath { get; set; }
}

public record SourceCodes
{
    public string? NEFT { get; set; }
    public string? SWIFTPAY { get; set; }
    public string? CIB { get; set; }
    public string? DOMTRF { get; set; }
    public string? DIVIDEND { get; set; }
}

public record ServiceWrapperConfiguration
{
    public string? ServiceUrl { get; set; }
    public string? ApiKeyName { get; set; }
    public string? ApiKeyValue { get; set; }
}

public record BatchIdConfiguration
{
    public string? BatchIdUrl { get; set; }
    public string? CBADateTimeUri { get; set; }
}

public record FilePathConfiguration
{
    public string? InboxPath { get; set; }
    public string? WIPPath { get; set; }
    public string? ArchivePath { get; set; }
    public string? TempPath { get; set; }
    public string? ErrorPath { get; set; }
    public string? DuplicateRecords { get; set; }
    public string? OriginBatchRecords { get; set; }
}

public record GLAccounts
{
    public string? VATGL { get; set; }
    public string? ClearingGL { get; set; }
    public string? IncomeGL { get; set; }
}

public record AccountBalLiveStatus
{
    public bool IsLIve { get; set; }
    public decimal CurrentBalance { get; set; }
}

public record ChargeLiveStatus
{
    public bool IsLIve { get; set; }
    public string? ChargeUrl { get; set; }
    public string? VatUrl { get; set; }
}

public record CallBackApi
{
    public string? AppId { get; set; }
    public string? ApiKey { get; set; }
    public List<CallBackApiUri>? ApiUri { get; set; }
}

public record CallBackApiUri
{
    public string? DividendUri { get; set; }
    public string? DomTrfUri { get; set; }
    public string? CIBUri { get; set; }
}

public class DomApiConfig
{
    public string? BaseUri { get; set; }
    public string? ApiKey { get; set; }
    public string? AppId { get; set; }
}

public class DomTrfApiPaths
{
    public string? NameEnquiry { get; set; }
}

public class DomApiConfiguration
{
    public DomApiConfig? DomApiConfig { get; set; }
    public DomTrfApiPaths? DomTrfApiPaths { get; set; }
}

public class RabbitMQSettings
{
    public string HostName { get; set; }
    public string VirtualHost { get; set; }
    public string Port { get; set; }
    public string QueueName { get; set; }
    public string Exchange { get; set; }
    public string RoutingKey { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}