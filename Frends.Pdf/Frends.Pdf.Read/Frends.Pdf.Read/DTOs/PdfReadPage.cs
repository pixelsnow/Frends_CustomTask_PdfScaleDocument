using System.Collections.Generic;

namespace Frends.Pdf.Read.DTOs
{
    /// <summary>
    /// Represents a single page of a PDF document.
    /// </summary>
    public class PdfReadPage
    {
        /// <summary>
        /// Page number within the PDF document.
        /// </summary>
        /// <example>1</example>
        public int Number { get; set; }

        /// <summary>
        /// Extracted plain text content of the page.
        /// </summary>
        /// <example>This is the first page</example>
        public string Text { get; set; }

        /// <summary>
        /// Images extracted from the page.
        /// </summary>
        /// <example>
        /// [ { "Format": "PNG", "Width": 200, "Height": 100 } ]
        /// </example>
        public List<PdfReadImage> Images { get; set; }
    }
}
