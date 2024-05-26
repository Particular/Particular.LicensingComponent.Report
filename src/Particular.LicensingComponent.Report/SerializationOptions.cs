namespace Particular.LicensingComponent.Report;

using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;

/// <summary>
/// Class to help with report serialization
/// </summary>
public static class SerializationOptions
{
    /// <summary>
    /// Serialize with relaxed escaping and indent
    /// </summary>
    public static readonly JsonSerializerOptions IndentedWithNoEscaping = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Serialize with relaxed escaping and without indent
    /// </summary>
    public static readonly JsonSerializerOptions NotIndentedWithNoEscaping = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}
