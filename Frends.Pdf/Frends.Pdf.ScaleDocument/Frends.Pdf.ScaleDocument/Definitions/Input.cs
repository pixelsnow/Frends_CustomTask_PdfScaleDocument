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

    /// <summary>
    /// If true, the file will only be scaled down in size. If any pages are smaller than the target size, they will be left unchanged. If false, all pages will be scaled to fit the target size.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool OnlyScaleDown { get; set; }
}
