namespace Particular.LicensingComponent.Report;

using System.Security.Cryptography;
using System.Text.Json;

/// <summary>
/// Report signing functionality
/// </summary>
public static class Signature
{
    static readonly string publicKeyText;

    static Signature()
    {
        var assembly = typeof(Signature).Assembly;
        var assemblyName = assembly.GetName().Name;
        using (var stream = assembly.GetManifestResourceStream($"{assemblyName}.public-key.pem"))
        using (var reader = new StreamReader(stream!))
        {
            publicKeyText = reader.ReadToEnd();
        }
    }

    /// <summary>
    /// Sign a report
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public static string SignReport(Report report)
    {
        var bytesToSign = JsonSerializer.SerializeToUtf8Bytes(report, SerializationOptions.NotIndentedWithNoEscaping);

        using (var rsa = RSA.Create())
        using (var sha = SHA512.Create())
        {
            rsa.ImportFromPem(publicKeyText);

            var hash = sha.ComputeHash(bytesToSign);

            var signature = rsa.Encrypt(hash, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }
    }
}