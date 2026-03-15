namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Actions if the output document already exists.
/// </summary>
public enum FileExistsActionEnum
{
    /// <summary>
    /// Throw error.
    /// </summary>
    Error,
    /// <summary>
    /// Overwrite the existing document.
    /// </summary>
    Overwrite,
    /// <summary>
    /// Rename the new document.
    /// </summary>
    Rename
}

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Page size options.
/// </summary>
public enum PageSizeEnum
{
    A0,
    A1,
    A2,
    A3,
    A4,
    A5,
    A6,
    B5,
    Ledger,
    Legal,
    Letter
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Options for page orientation.
/// </summary>
public enum PageOrientationEnum
{
    Portrait,
    Landscape
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Element which will be added to the document.
/// </summary>
public enum ElementType
{
    Paragraph,
    Image,
    PageBreak,
    Header,
    Footer,
    Table
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Alignment of paragraph.
/// </summary>
public enum ParagraphAlignmentEnum
{
    Left,
    Center,
    Justify,
    Right
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Alignment for image.
/// </summary>
public enum ImageAlignmentEnum
{
    Left,
    Center,
    Right
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Options for paragraph font.
/// </summary>
public enum FontStyleEnum
{
    Regular,
    Bold,
    Italic,
    BoldItalic,
    Underline
}
#pragma warning restore CS1591

/// <summary>
/// Options for header/footer input.
/// </summary>
public enum HeaderFooterStyleEnum
{
    /// <summary>
    /// Add only text.
    /// </summary>
    Text,
    /// <summary>
    /// Add text and page numbers.
    /// </summary>
    TextPagenum,
    /// <summary>
    /// Add text and image.
    /// </summary>
    LogoText,
    /// <summary>
    /// Add text, image and page numbers.
    /// </summary>
    LogoTextPagenum
}

/// <summary>
/// Options for table type.
/// </summary>
public enum TableTypeEnum
{
    /// <summary>
    /// Normal table in the document.
    /// </summary>
    Table,
    /// <summary>
    /// Add table to header.
    /// </summary>
    Header,
    /// <summary>
    /// Add table to footer.
    /// </summary>
    Footer
}

/// <summary>
/// Type of data which will be added to the table columns.
/// </summary>
public enum TableColumnType
{
    /// <summary>
    /// Text data.
    /// </summary>
    Text,
    /// <summary>
    /// Images.
    /// </summary>
    Image,
    /// <summary>
    /// Page numbers.
    /// </summary>
    PageNum
}

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Table style selector for changing style to different borders.
/// </summary>
public enum TableBorderStyle
{
    None,
    Top,
    Bottom,
    All
}
#pragma warning restore CS1591
