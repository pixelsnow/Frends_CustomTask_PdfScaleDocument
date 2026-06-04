namespace Frends.Pdf.ScaleDocument.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }
    
    /// <summary>
    /// Additional informational text from the task run.
    /// </summary>
    public string Info { get; set; }

    /// <summary>
    /// Returns info log about the execution
    /// </summary>
    public string Info { get; set; }

    /// <summary>
    /// Base 64 binary of the scaled PDF file.
    /// </summary>
    public string ResultBase64 { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception AdditionalInfo } }</example>
    public Error Error { get; set; }
}