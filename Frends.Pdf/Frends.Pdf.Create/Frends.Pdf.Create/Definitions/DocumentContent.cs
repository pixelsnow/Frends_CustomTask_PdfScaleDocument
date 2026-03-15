using System.ComponentModel;

namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Class for wrapping page contents together.
/// </summary>
public class DocumentContent
{
    /// <summary>
    /// Document content.
    /// </summary>
    /// <example>Array of PageContentElements</example>
    [DisplayName("Document Content")]
    public PageContentElement[] Contents { get; set; }
}
