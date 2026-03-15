namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Class for table styles.
/// </summary>
public class TableStyle
{
    /// <summary>
    /// Font family.
    /// </summary>
    /// <example>Times New Roman</example>
    public string FontFamily { get; set; }

    /// <summary>
    /// Font size in points.
    /// </summary>
    /// <example>8</example>
    public double FontSizeInPt { get; set; }

    /// <summary>
    /// Font style.
    /// </summary>
    /// <example>Regular</example>
    public FontStyleEnum FontStyle { get; set; }

    /// <summary>
    /// Spacing between words in points.
    /// </summary>
    /// <example>3</example>
    public double LineSpacingInPt { get; set; }

    /// <summary>
    /// Spacing before line in points.
    /// </summary>
    /// <example>0</example>
    public double SpacingBeforeInPt { get; set; }

    /// <summary>
    /// Spacing after line in points.
    /// </summary>
    /// <example>0</example>
    public double SpacingAfterInPt { get; set; }

    /// <summary>
    /// Border width in points.
    /// </summary>
    /// <example>0</example>
    public double BorderWidthInPt { get; set; }

    /// <summary>
    /// Style of borders.
    /// </summary>
    /// <example>None</example>
    public TableBorderStyle BorderStyle { get; set; }
}
