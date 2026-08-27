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
    public static Report? ReadAndValidate(Stream stream, out ReportValidationResult validationResult)
    {
        using var doc = JsonDocument.Parse(stream);
        var root = doc.RootElement;

        if (!root.TryGetProperty(nameof(SignedReport.ReportData), out var reportDataElement))
        {
            validationResult = new ReportValidationResult
            {
                IsValid = false,
                InvalidReason = "Could not find ReportData property"
            };
            return null;
        }

        validationResult = ValidateSignature(root, reportDataElement);

        return reportDataElement.Deserialize<Report>();
    }

    static ReportValidationResult ValidateSignature(JsonElement root, JsonElement reportDataElement)
    {
        // NOTE: In .NET 9 we can get the bytes directly from the JsonElement, but in .NET 8 we have to get the raw text and convert it to bytes ourselves.
        var reportBytes = MinifyJsonElementToBytes(reportDataElement);

        var reportId = Convert.ToHexString(SHA1.HashData(reportBytes));
        var validationResult = new ReportValidationResult
        {
            ReportId = reportId
        };

        if (!root.TryGetProperty(nameof(SignedReport.Signature), out var signatureElement))
        {
            validationResult.InvalidReason = "Could not find signature";
            return validationResult;
        }

        var signature = signatureElement.GetString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(signature))
        {
            validationResult.InvalidReason = "Signature is empty";
            return validationResult;
        }

        // Validate signature
        var pemData = Environment.GetEnvironmentVariable("THROUGHPUT_REPORT_PRIVATEKEY_PEM");
        if (string.IsNullOrWhiteSpace(pemData))
        {
            validationResult.InvalidReason = "No private key available to validate signature";
            return validationResult;
        }

        byte[] signatureBytes;
        try
        {
            signatureBytes = Convert.FromBase64String(signature);
        }
        catch (FormatException)
        {
            // NOTE: The signature is not valid base64, which means it is invalid. We will return false for IsValid in this case.
            validationResult.InvalidReason = "Signature could not be decoded";
            return validationResult;
        }

        var correctSignature = Convert.ToBase64String(SHA512.HashData(reportBytes));

        using var rsa = RSA.Create();

        rsa.ImportFromPem(pemData);
        var decryptedHash = rsa.Decrypt(signatureBytes, RSAEncryptionPadding.Pkcs1);
        var decryptedSignature = Convert.ToBase64String(decryptedHash);

        if (correctSignature == decryptedSignature)
        {
            validationResult.IsValid = true;
        }
        else
        {
            validationResult.InvalidReason = "Signature does not match report data";
        }

        return validationResult;
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
