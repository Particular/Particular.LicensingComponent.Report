namespace Particular.LicensingComponent.Report;

using System;

/// <summary>
/// Exception thrown if private key not available
/// </summary>
public class NoPrivateKeyException : Exception
{
    /// <summary>
    /// ReportId of the report being processed
    /// </summary>
    public string ReportId { get; }

    internal NoPrivateKeyException(string reportId)
        : base($"Report private key not found to validate report {reportId}")
    {
        ReportId = reportId;
    }

}
