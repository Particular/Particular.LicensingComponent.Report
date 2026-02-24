namespace Particular.LicensingComponent.Report.Utility;
/// <summary>
/// Provides functionality to mask specified words in a given string with unique replacement values.
/// </summary>
public class Masker
{
    readonly (string Mask, string Replacement)[] masks;

    /// <summary>
    /// Initializes a new instance of the Masker class with the specified words to be masked.
    /// </summary>
    public Masker(string[] wordsToMask)
    {
        var number = 0;
        masks = [.. wordsToMask
            .Select(mask =>
            {
                number++;
                return (mask, $"REDACTED{number}");
            })];
    }

    /// <summary>
    /// Replaces all occurrences of specified substrings in the input string with their corresponding masked values
    /// using a predefined set of masks.
    /// </summary>
    public string Mask(string stringToMask)
    {
        var result = stringToMask;
        foreach (var (mask, replacement) in masks)
        {
            result = result.Replace(mask, replacement, StringComparison.OrdinalIgnoreCase);
        }
        return result;
    }
}