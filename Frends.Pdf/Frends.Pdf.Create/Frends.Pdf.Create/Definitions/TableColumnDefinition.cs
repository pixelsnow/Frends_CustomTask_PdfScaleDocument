namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Definition-class for table columns.
/// </summary>
public class TableColumnDefinition
{
    /// <summary>
    /// Name of the column.
    /// </summary>
    /// <example>FirstColumn</example>
    public string Name { get; set; }

    /// <summary>
    /// Column width in centimeters.
    /// </summary>
    /// <example>5</example>
    public double WidthInCm { get; set; }

    /// <summary>
    /// Column height in centimeters.
    /// </summary>
    /// <example>3</example>
    public double HeightInCm { get; set; }

    /// <summary>
    /// Type of column.
    /// </summary>
    /// <example>Text</example>
    public TableColumnType Type { get; set; }
}
