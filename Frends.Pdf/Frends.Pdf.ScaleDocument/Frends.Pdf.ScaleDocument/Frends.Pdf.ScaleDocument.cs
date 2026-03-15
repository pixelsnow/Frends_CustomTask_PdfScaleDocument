using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Frends.Pdf.ScaleDocument.Definitions;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Frends.Pdf.ScaleDocument
{
    /// <summary>
    /// Converts an A3 PDF into an A4-scaled version using PdfSharpCore (XPdfForm).
    /// Uses only the first file in InputFilePaths as the source.
    /// </summary>
    public static class Pdf
    {
        public static Result ScaleDocument(
            [PropertyTab] Input input,
            [PropertyTab] Options options,
            CancellationToken cancellationToken)
        {
            try
            {
                if (input.InputFilePaths == null || input.InputFilePaths.Length == 0)
                    throw new Exception("No input file specified.");

                if (File.Exists(input.DestinationFilePath))
                    throw new Exception("Destination file already exists.");

                if (Path.GetExtension(input.DestinationFilePath)?.Equals(".pdf", StringComparison.OrdinalIgnoreCase) != true)
                    throw new Exception("Destination file must have .pdf extension.");

                var sourcePath = input.InputFilePaths[0];

                // Load external PDF as a form once; switch PageNumber per iteration.
                using var form = XPdfForm.FromFile(sourcePath);

                using var output = new PdfDocument();

                // A4 dimensions in points (72 dpi * 8.27 x 11.69 inches)
                const double A4WidthPt = 595.28;  // 210 mm
                const double A4HeightPt = 841.89; // 297 mm

                for (var pageIndex = 0; pageIndex < form.PageCount; pageIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Select the source page (1-based)
                    form.PageNumber = pageIndex + 1;

                    // Determine target A4 orientation to match source page orientation
                    var srcWidth = form.PointWidth;   // width of the selected source page
                    var srcHeight = form.PointHeight; // height of the selected source page
                    var landscape = srcWidth > srcHeight;

                    var targetWidth = landscape ? A4HeightPt : A4WidthPt;
                    var targetHeight = landscape ? A4WidthPt : A4HeightPt;

                    var newPage = output.AddPage();
                    newPage.Width = targetWidth;
                    newPage.Height = targetHeight;

                    using var gfx = XGraphics.FromPdfPage(newPage);

                    // Compute uniform scale to fit inside A4 page
                    var scaleX = targetWidth / srcWidth;
                    var scaleY = targetHeight / srcHeight;
                    var scale = Math.Min(scaleX, scaleY);

                    // Center the scaled content
                    var drawWidth = srcWidth * scale;
                    var drawHeight = srcHeight * scale;
                    var dx = (targetWidth - drawWidth) / 2.0;
                    var dy = (targetHeight - drawHeight) / 2.0;

                    // Draw the selected source page into the A4 page
                    gfx.DrawImage(form, new XRect(dx, dy, drawWidth, drawHeight));
                }

                var dir = Path.GetDirectoryName(input.DestinationFilePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                output.Save(input.DestinationFilePath);

                return new Result { Success = true, Error = null };
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                return Helpers.ErrorHandler.Handle(e, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
            }
        }
    }
}