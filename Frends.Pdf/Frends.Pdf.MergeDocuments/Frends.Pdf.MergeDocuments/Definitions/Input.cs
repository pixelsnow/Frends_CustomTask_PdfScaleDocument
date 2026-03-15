using System.ComponentModel;

namespace Frends.Pdf.MergeDocuments.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Paths to files to merge.
    /// </summary>
    /// <example>["C:/files/foo.pdf", "C:/files/bar.pdf"]</example>
    public string[] InputFilePaths { get; set; }

    /// <summary>
    /// Path where to save merged Pdf.
    /// </summary>
    /// <example>"C:/files/merged.pdf"</example>
    [DefaultValue("")]
    public string DestinationFilePath { get; set; }
}
