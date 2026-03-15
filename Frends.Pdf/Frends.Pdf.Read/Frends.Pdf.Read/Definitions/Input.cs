using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Pdf.Read.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Full path to the PDF file.
    /// </summary>
    /// <example>C:\temp\file.pdf</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string FilePath { get; set; }
}
