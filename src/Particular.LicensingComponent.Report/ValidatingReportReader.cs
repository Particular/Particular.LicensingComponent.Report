namespace Particular.LicensingComponent.Report;

using System.Buffers;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;

/// <summary>
/// Helper class to read and validate a signed report
/// </summary>
public static class ValidatingReportReader
{
    /// <summary>
    /// Reads and validates a signed report from a stream
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="validationResult"></param>
    public static SignedReport? ReadAndValidate(Stream stream, out ReportValidationResult validationResult)
    {
        using var doc = JsonDocument.Parse(stream);
        var root = doc.RootElement;

        validationResult = ValidateSignature(root);

        return root.Deserialize<SignedReport>();
    }

    static ReportValidationResult ValidateSignature(JsonElement root)
    {
        if (!root.TryGetProperty(nameof(SignedReport.ReportData), out var reportDataElement))
        {
            return ReportValidationResult.Invalid(null, "Could not find ReportData property");
        }

        // NOTE: In .NET 9 we can get the bytes directly from the JsonElement, but in .NET 8 we have to get the raw text and convert it to bytes ourselves.
        var reportBytes = MinifyJsonElementToBytes(reportDataElement);

        var reportId = Convert.ToHexString(SHA1.HashData(reportBytes));

        if (!root.TryGetProperty(nameof(SignedReport.Signature), out var signatureElement))
        {
            return ReportValidationResult.Invalid(reportId, "Could not find signature");
        }

        var signature = signatureElement.GetString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(signature))
        {
            return ReportValidationResult.Invalid(reportId, "Signature is empty");
        }

        // Validate signature
        var pemData = Environment.GetEnvironmentVariable("THROUGHPUT_REPORT_PRIVATEKEY_PEM");
        if (string.IsNullOrWhiteSpace(pemData))
        {
            return ReportValidationResult.Invalid(reportId, "No private key available to validate signature");
        }

        byte[] signatureBytes;
        try
        {
            signatureBytes = Convert.FromBase64String(signature);
        }
        catch (FormatException)
        {
            // NOTE: The signature is not valid base64, which means it is invalid. We will return false for IsValid in this case.
            return ReportValidationResult.Invalid(reportId, "Signature could not be decoded");
        }

        var correctSignature = Convert.ToBase64String(SHA512.HashData(reportBytes));

        using var rsa = RSA.Create();

        rsa.ImportFromPem(pemData);
        var decryptedHash = rsa.Decrypt(signatureBytes, RSAEncryptionPadding.Pkcs1);
        var decryptedSignature = Convert.ToBase64String(decryptedHash);

        return (correctSignature == decryptedSignature)
            ? ReportValidationResult.Valid(reportId)
            : ReportValidationResult.Invalid(reportId, "Signature does not match report data");
    }

    static readonly JsonWriterOptions MinifyOptions = new()
    {
        Indented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static byte[] MinifyJsonElementToBytes(JsonElement element)
    {
        var bufferWriter = new ArrayBufferWriter<byte>(initialCapacity: 4096);
        using var memoryStream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(bufferWriter, MinifyOptions))
        {
            element.WriteTo(writer);
        }

        return bufferWriter.WrittenMemory.ToArray();
    }
}
