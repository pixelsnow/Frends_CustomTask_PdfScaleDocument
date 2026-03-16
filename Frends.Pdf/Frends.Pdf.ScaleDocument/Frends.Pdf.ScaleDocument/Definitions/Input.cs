using System.ComponentModel;

namespace Frends.Pdf.ScaleDocument.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Path to file to scale.
    /// </summary>
    /// <example>"C:/files/source.pdf"</example>
    public string InputFilePath { get; set; }

    /// <summary>
    /// Path where to save scaled Pdf.
    /// </summary>
    /// <example>"C:/files/result.pdf"</example>
    [DefaultValue("")]
    public string DestinationFilePath { get; set; }

    /// <summary>
    /// Document page size to which to scale.
    /// </summary>
    /// <example>A4</example>
    [DefaultValue(PageSizeEnum.A4)]
    public PageSizeEnum Size { get; set; }
}
