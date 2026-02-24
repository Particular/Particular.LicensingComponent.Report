namespace Particular.LicensingComponent.Report.Utility;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides static methods for computing one-way hashes using the SHA3-256 algorithm.
/// </summary>
public static class OneWayHasher
{
    /// <summary>
    /// Calculates the SHA3-256 hash of the specified input string and returns the result as a hexadecimal string.
    /// </summary>
    public static string CalculateOneWayHash(string input)
        => BitConverter.ToString(SHA3_256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", "");
}