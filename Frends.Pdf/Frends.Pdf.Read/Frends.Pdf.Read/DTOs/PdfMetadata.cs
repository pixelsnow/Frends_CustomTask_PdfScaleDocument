namespace Frends.Pdf.Read.DTOs
{
    /// <summary>
    /// Contains metadata information of a PDF document.
    /// </summary>
    public class PdfMetadata
    {
        /// <summary>
        /// Title of the PDF document.
        /// </summary>
        /// <example>Sample PDF</example>
        public string Title { get; set; }

        /// <summary>
        /// Author of the PDF document.
        /// </summary>
        /// <example>John Doe</example>
        public string Author { get; set; }

        /// <summary>
        /// Total number of pages in the document.
        /// </summary>
        /// <example>3</example>
        public int PageCount { get; set; }
    }
}
