namespace Frends.Pdf.ScaleDocument.Definitions;

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
    Rename,
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
    Letter,
}
#pragma warning restore CS1591

#pragma warning disable CS1591 // Self-explanatory enums.
/// <summary>
/// Options for page orientation.
/// </summary>
public enum PageOrientationEnum
{
    Portrait,
    Landscape,
}
#pragma warning restore CS1591