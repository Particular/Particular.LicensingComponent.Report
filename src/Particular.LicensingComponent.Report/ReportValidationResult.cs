namespace Particular.LicensingComponent.Report;

/// <summary>
/// Report validator
/// </summary>
public class ReportValidationResult
{
    /// <summary>
    /// Is the report valid
    /// </summary>
    public bool IsValid { get; internal set; }

    /// <summary>
    /// Reason why the report is invalid, if applicable
    /// </summary>
    public string? InvalidReason { get; internal set; }

    /// <summary>
    /// Report id of the report being validated
    /// </summary>
    public string? ReportId { get; internal set; }


    internal static ReportValidationResult Valid(string reportId) => new()
    {
        IsValid = true,
        ReportId = reportId
    };

    internal static ReportValidationResult Invalid(string? reportId, string reason) => new()
    {
        IsValid = false,
        InvalidReason = reason,
        ReportId = reportId
    };
}
