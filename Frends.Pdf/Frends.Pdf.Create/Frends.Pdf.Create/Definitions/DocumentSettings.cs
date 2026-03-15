using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Class for adding settings for the PDF document.
/// </summary>
public class DocumentSettings
{
    /// <summary>
    /// Optional PDF document title.
    /// </summary>
    /// <example>Very important document.</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Title { get; set; }

    /// <summary>
    /// Optional PDF document Author.
    /// </summary>
    /// <example>Erik Example</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Author { get; set; }

    /// <summary>
    /// Document page size.
    /// </summary>
    /// <example>A4</example>
    [DefaultValue(PageSizeEnum.A4)]
    public PageSizeEnum Size { get; set; }

    /// <summary>
    /// Page orientation.
    /// </summary>
    /// <example>Portrait</example>
    [DefaultValue(PageOrientationEnum.Portrait)]
    public PageOrientationEnum Orientation { get; set; }

    /// <summary>
    /// Page margin left in CM.
    /// </summary>
    /// <example>2.5</example>
    [DefaultValue(2.5)]
    public double MarginLeftInCm { get; set; }

    /// <summary>
    /// Page margin top in CM.
    /// </summary>
    /// <example>2</example>
    [DefaultValue(2)]
    public double MarginTopInCm { get; set; }

    /// <summary>
    /// Page margin right in CM.
    /// </summary>
    /// <example>2.5</example>
    [DefaultValue(2.5)]
    public double MarginRightInCm { get; set; }

    /// <summary>
    /// Page margin bottom in CM.
    /// </summary>
    /// <example>2</example>
    [DefaultValue(2)]
    public double MarginBottomInCm { get; set; }

}
