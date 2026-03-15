using System.Collections.Generic;

namespace Frends.Pdf.Read.DTOs
{
    /// <summary>
    /// Represents the extracted contents of a PDF document.
    /// </summary>
    public class PdfReadDocument
    {
        /// <summary>
        /// Document-level metadata extracted from the PDF.
        /// </summary>
        /// <example>
        /// { "Title": "Sample PDF", "Author": "John Doe", "PageCount": 2 }
        /// </example>
        public PdfMetadata Metadata { get; set; }

        /// <summary>
        /// Pages extracted from the PDF document.
        /// </summary>
        /// <example>
        /// [ { "Number": 1, "Text": "Hello world", "Images": [] } ]
        /// </example>
        public List<PdfReadPage> Pages { get; set; }
    }
}
