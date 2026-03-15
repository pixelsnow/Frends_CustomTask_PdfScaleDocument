using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Frends.Pdf.MergeDocuments.Definitions;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace Frends.Pdf.MergeDocuments;

/// <summary>
/// Task class.
/// </summary>
public static class Pdf
{
    /// <summary>
    /// Task to merge multiple Pdf documents into one.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Pdf-MergeDocuments)
    /// </summary>
    /// <param name="input">Information about input and output paths</param>
    /// <param name="options">Exception settings</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>object { bool Success, object Error { string Message, exception AdditionalInfo } }</returns>
    public static Result MergeDocuments(
        [PropertyTab] Input input,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (File.Exists(input.DestinationFilePath)) throw new Exception("Destination file already exists");
            if (Path.GetExtension(input.DestinationFilePath) != ".pdf")
                throw new Exception("Destination file must have .pdf extension");

            using var outputDocument = new PdfDocument();
            foreach (var filePath in input.InputFilePaths)
            {
                using var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                foreach (var page in inputDocument.Pages)
                {
                    outputDocument.AddPage(page);
                }
            }

            var dir = Path.GetDirectoryName(input.DestinationFilePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            outputDocument.Save(input.DestinationFilePath);

            return new Result { Success = true, Error = null, };
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            return Helpers.ErrorHandler.Handle(e, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
        }
    }
}