namespace Frends.Pdf.Read.DTOs
{
    /// <summary>
    /// Represents an image extracted from a PDF page.
    /// </summary>
    public class PdfReadImage
    {
        /// <summary>
        /// Image format.
        /// </summary>
        /// <example>PNG</example>
        public string Format { get; set; }

        /// <summary>
        /// Raw image data as byte array.
        /// </summary>
        /// <example>
        /// "base64-encoded byte array"
        /// </example>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Width of image in points.
        /// </summary>
        /// <example>200</example>
        public double Width { get; set; }

        /// <summary>
        /// Height of image in points.
        /// </summary>
        /// <example>400</example>
        public double Height { get; set; }
    }
}
