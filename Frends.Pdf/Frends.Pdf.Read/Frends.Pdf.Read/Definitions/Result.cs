namespace Frends.Pdf.Read.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="success">True if the operation succeeded.</param>
    /// <param name="json">JSON representation of the extracted PDF content.</param>
    /// <param name="error">Error details if the operation failed.</param>
    public Result(bool success, string json, Error error = null)
    {
        Success = success;
        Json = json;
        Error = error;
    }

    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// JSON representation of the extracted PDF content, including document metadata and page-level content.
    /// Text is extracted in reading order; tables and columns are flattened.
    /// Images (if IncludeImages is true) are returned as a byte array, with its width and height.
    /// If the extraction library could convert the image, Format is "PNG"; otherwise Format is "RAW".
    /// A single visible image may produce multiple extracted images due to transparency masks or internal PDF structure.
    /// </summary>
    /// <example>
    /// {
    ///   "Metadata": {
    ///     "Title": "Example PDF",
    ///     "Author": "John Doe",
    ///     "PageCount": 2
    ///   },
    ///   "Pages": [
    ///     {
    ///       "Number": 1,
    ///       "Text": "Hello world",
    ///       "Images": []
    ///     },
    ///     {
    ///       "Number": 2,
    ///       "Text": "Second page text",
    ///       "Images": []
    ///     }
    ///   ]
    /// }
    /// </example>
    public string Json { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
