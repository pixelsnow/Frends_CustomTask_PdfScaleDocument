using System.ComponentModel;

namespace Frends.Pdf.ScaleDocument.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Paths to file to scale.
    /// </summary>
    /// <example>["C:/files/foo.pdf", "C:/files/bar.pdf"]</example>
    public string[] InputFilePaths { get; set; }

    /// <summary>
    /// Path where to save scaled Pdf.
    /// </summary>
    /// <example>"C:/files/merged.pdf"</example>
    [DefaultValue("")]
    public string DestinationFilePath { get; set; }
}
