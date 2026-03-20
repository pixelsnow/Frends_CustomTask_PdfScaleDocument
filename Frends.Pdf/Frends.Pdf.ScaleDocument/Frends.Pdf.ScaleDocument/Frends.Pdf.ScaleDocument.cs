using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Frends.Pdf.ScaleDocument.Definitions;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Frends.Pdf.ScaleDocument
{
    /// <summary>
    /// Creates a new scaled PDF file from a given source PDF file. Each page of the source PDF is scaled to fit inside the target size while maintaining the aspect ratio. The output PDF will have the same number of pages as the source, but each page will be resized in either portrait or landscape orientation depending on the original page orientation.
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
                if (input.InputBase64 == null)
                    throw new Exception("InputBase64 is not given.");

                byte[] inputBytes;
                try
                {
                    inputBytes = Convert.FromBase64String(input.InputBase64);
                }
                catch
                {
                    throw new Exception("InputBase64 is not valid Base64.");
                }

                using var inputStream = new MemoryStream(inputBytes);
                using var form = XPdfForm.FromStream(inputStream);
                using var output = new PdfDocument();

                // Get the selected page size.
                PageSetup.GetPageSize(input.Size.ConvertEnum<PageFormat>(), out Unit width, out Unit height);

                // Target dimensions in points
                double targetSizeWidthPt = width.Point;
                double targetSizeHeightPt = height.Point;

                for (var pageIndex = 0; pageIndex < form.PageCount; pageIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Select the source page
                    form.PageNumber = pageIndex + 1;

                    // Determine source page orientation
                    var srcWidth = form.PointWidth;
                    var srcHeight = form.PointHeight;
                    var landscape = srcWidth > srcHeight;

                    // Calculate target dimensions based on orientation
                    var targetWidth = landscape ? targetSizeHeightPt : targetSizeWidthPt;
                    var targetHeight = landscape ? targetSizeWidthPt : targetSizeHeightPt;

                    // This is needed to avoid scaling up pages that are smaller than the target size when OnlyScaleDown is true
                    bool pageIsSmallerThanTarget = srcWidth <= targetWidth && srcHeight <= targetHeight;

                    if (input.OnlyScaleDown && pageIsSmallerThanTarget)
                    {
                        // Copy the page exactly as is without scaling
                        PdfPage newPage = output.AddPage();
                        newPage.Width = XUnit.FromPoint(srcWidth);
                        newPage.Height = XUnit.FromPoint(srcHeight);
                        using var gfx = XGraphics.FromPdfPage(newPage);
                        gfx.DrawImage(form, new XRect(0, 0, srcWidth, srcHeight));
                    }
                    else
                    {
                        // Scale the page to fit inside the target size
                        PdfPage newPage = output.AddPage();
                        newPage.Width = XUnit.FromPoint(targetWidth);
                        newPage.Height = XUnit.FromPoint(targetHeight);

                        using var gfx = XGraphics.FromPdfPage(newPage);

                        // Compute scale to fit inside the target dimensions while maintaining aspect ratio
                        var scaleX = targetWidth / srcWidth;
                        var scaleY = targetHeight / srcHeight;
                        var scale = Math.Min(scaleX, scaleY);

                        // Center the scaled content
                        var drawWidth = srcWidth * scale;
                        var drawHeight = srcHeight * scale;
                        var dx = (targetWidth - drawWidth) / 2.0;
                        var dy = (targetHeight - drawHeight) / 2.0;

                        // Draw the scaled page
                        gfx.DrawImage(form, new XRect(dx, dy, drawWidth, drawHeight));
                    }
                }

                using var outputStream = new MemoryStream();
                output.Save(outputStream);
                string resultBase64 = Convert.ToBase64String(outputStream.ToArray());

                return new Result { Success = true, ResultBase64 = resultBase64, Error = null };
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                return Helpers.ErrorHandler.Handle(e, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
            }
        }
    }
}