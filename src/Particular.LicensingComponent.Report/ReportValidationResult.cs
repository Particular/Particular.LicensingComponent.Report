namespace Particular.LicensingComponent.Report;

using System.Security.Cryptography;
using System.Text.Json;

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
    /// Report id of the report being validated
    /// </summary>
    public string ReportId { get; }

    internal ReportValidationResult(string reportId)
    {
        ReportId = reportId;
        IsValid = true;
    }

    /// <summary>
    /// Method that tests whether the signed report is valid
    /// </summary>
    /// <param name="signedReport"></param>
    /// <exception cref="NoPrivateKeyException"></exception>
    public ReportValidationResult(SignedReport signedReport)
    {
        var reserializedReportBytes = JsonSerializer.SerializeToUtf8Bytes(signedReport.ReportData, SerializationOptions.NotIndentedWithNoEscaping);

        ReportId = Convert.ToHexString(SHA1.HashData(reserializedReportBytes));

        if (signedReport?.Signature is null)
        {
            return;
        }

        var pemData = Environment.GetEnvironmentVariable("THROUGHPUT_REPORT_PRIVATEKEY_PEM") ?? throw new NoPrivateKeyException(ReportId);

        byte[] signatureBytes;
        try
        {
            signatureBytes = Convert.FromBase64String(signedReport.Signature);
        }
        catch (FormatException)
        {
            return;
        }

        var correctSignature = Convert.ToBase64String(SHA512.HashData(reserializedReportBytes));

        using var rsa = RSA.Create();

        rsa.ImportFromPem(pemData);
        var decryptedHash = rsa.Decrypt(signatureBytes, RSAEncryptionPadding.Pkcs1);
        var decryptedSignature = Convert.ToBase64String(decryptedHash);

        IsValid = correctSignature == decryptedSignature;
    }
}
