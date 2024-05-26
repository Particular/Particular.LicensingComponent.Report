namespace Particular.LicensingComponent.Report;

using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

//NOTE do not change fields to be nullable as this needs to be compatible with older versions of the report

/// <summary>
/// SignedReport
/// </summary>
public record SignedReport
{
    /// <summary>
    /// Report content
    /// </summary>
    public Report ReportData { get; init; }

    /// <summary>
    /// Report signature
    /// </summary>
    public string Signature { get; init; }
}


/// <summary>
/// Report content
/// </summary>
public record Report
{
    /// <summary>
    /// Customer name the report is for
    /// </summary>
    public string CustomerName { get; init; }

    /// <summary>
    /// Message transport used
    /// </summary>
    public string MessageTransport { get; init; }

    /// <summary>
    /// Report method
    /// </summary>
    public string ReportMethod { get; init; }

    /// <summary>
    /// Tool used to generate the report
    /// </summary>
    public string? ToolType { get; init; }

    /// <summary>
    /// Version of the tool used to generate the report
    /// </summary>
    public string ToolVersion { get; init; }

    /// <summary>
    /// Prefix used in filtering out broker queues
    /// </summary>
    public string? Prefix { get; init; }

    /// <summary>
    /// Scope type of the broker
    /// </summary>
    public string ScopeType { get; init; }

    /// <summary>
    /// Report period start time
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Report period end time
    /// </summary>
    public DateTimeOffset EndTime { get; init; }

    /// <summary>
    /// Not necessarily the difference between Start/End time. ASB for example collects
    /// 30 days worth of data and reports the max daily throughput for one 24h period.
    /// </summary>
    public TimeSpan ReportDuration { get; set; }

    /// <summary>
    /// Detected queues
    /// </summary>
    public QueueThroughput[] Queues { get; init; }

    /// <summary>
    /// Total throughput for all queues
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)] // Must be serialized even if 0 to maintain compatibility with old report signatures
    public long TotalThroughput { get; init; }

    /// <summary>
    /// Total number of detected queues
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)] // Must be serialized even if 0 to maintain compatibility with old report signatures
    public int TotalQueues { get; init; }

    /// <summary>
    /// Any ignored queues
    /// </summary>
    public string[] IgnoredQueues { get; init; }

    /// <summary>
    /// Information about the environment, ie ServicePulse version, Audit instances versipns etc
    /// </summary>
    public EnvironmentInformation EnvironmentInformation { get; set; }
}

/// <summary>
/// Queue throughput information
/// </summary>
public record QueueThroughput
{
    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>
    /// Queue max daily throughput
    /// </summary>
    public long? Throughput { get; set; }

    /// <summary>
    /// Is this a sendonly queue
    /// </summary>
    public bool NoDataOrSendOnly { get; init; }

    /// <summary>
    /// Indicators for why this queue is a valid NServiceBus endpoint
    /// </summary>
    public string[] EndpointIndicators { get; init; }

    /// <summary>
    /// Indicators from customer as to why this queue should not be counted 
    /// </summary>
    public string? UserIndicator { get; init; }

    /// <summary>
    /// Broker specific information
    /// </summary>
    public string Scope { get; init; }

    /// <summary>
    /// Breakdown of daily throughput for each queue obtained from the broker
    /// </summary>
    public DailyThroughput[] DailyThroughputFromBroker { get; init; }

    /// <summary>
    /// Breakdown of daily throughput for each queue obtained from audit instances
    /// </summary>
    public DailyThroughput[] DailyThroughputFromAudit { get; init; }

    /// <summary>
    /// Breakdown of daily throughput for each queue obtained from monitoring
    /// </summary>
    public DailyThroughput[] DailyThroughputFromMonitoring { get; init; }
}

/// <summary>
/// Environment information that can be useful for data analysis
/// </summary>
public record EnvironmentInformation
{
    /// <summary>
    /// Information about any audit instances, such as versions and transports used
    /// </summary>
    public AuditServicesData AuditServicesData { get; set; }

    /// <summary>
    /// Other environment information, such as ServicePulse version
    /// </summary>
    public Dictionary<string, string> EnvironmentData { get; set; }
}

/// <summary>
/// Daily throughput record for a queue
/// </summary>
public record DailyThroughput
{
    /// <summary>
    /// Date of throughput
    /// </summary>
    public DateOnly DateUTC { get; init; }

    /// <summary>
    /// Message count
    /// </summary>
    public long MessageCount { get; init; }
}

/// <summary>
/// Data about audit services
/// </summary>
/// <param name="Versions"></param>
/// <param name="Transports"></param>
public record AuditServicesData(Dictionary<string, int> Versions, Dictionary<string, int> Transports)
{
}
