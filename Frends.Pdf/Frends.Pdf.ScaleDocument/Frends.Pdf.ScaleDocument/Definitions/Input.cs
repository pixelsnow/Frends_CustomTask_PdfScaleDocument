using System.ComponentModel;

namespace Frends.Pdf.ScaleDocument.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Base64 encoded binary data of the file to scale.
    /// </summary>
    public string InputBase64 { get; set; }

    /// <summary>
    /// Document page size to which to scale.
    /// </summary>
    /// <example>A4</example>
    [DefaultValue(PageSizeEnum.A4)]
    public PageSizeEnum Size { get; set; }
}
