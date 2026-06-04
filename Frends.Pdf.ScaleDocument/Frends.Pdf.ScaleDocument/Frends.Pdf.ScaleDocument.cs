using System;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Threading;
using Frends.Pdf.ScaleDocument.Definitions;
using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

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
                string logInfo = "";

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
                // Also open a PdfDocument to read page rotation values
                using var inputDoc = PdfReader.Open(new MemoryStream(inputBytes), PdfDocumentOpenMode.Import);
                using var output = new PdfDocument();

                // Also open a PdfDocument to read page rotation values
                using var inputDoc = PdfReader.Open(new MemoryStream(inputBytes), PdfDocumentOpenMode.Import);

                // Get the selected page size.
                PageSetup.GetPageSize(input.Size.ConvertEnum<PageFormat>(), out Unit width, out Unit height);

                // Target dimensions in points
                double targetSizeWidthPt = width.Point;
                double targetSizeHeightPt = height.Point;
                // Log target size (only)
                logInfo += $"Target size (points) width={targetSizeWidthPt}, height={targetSizeHeightPt}\n";

                static void DrawRotatedForm(XGraphics gfx, XPdfForm form, int rotate, double scale, double dx, double dy)
                {
                    var scaledWidth = form.PointWidth * scale;
                    var scaledHeight = form.PointHeight * scale;

                    gfx.Save();

                    switch (rotate)
                    {
                        case 90:
                            gfx.TranslateTransform(dx + scaledHeight, dy);
                            gfx.RotateTransform(90);
                            break;
                        case 180:
                            gfx.TranslateTransform(dx + scaledWidth, dy + scaledHeight);
                            gfx.RotateTransform(180);
                            break;
                        case 270:
                            gfx.TranslateTransform(dx, dy + scaledWidth);
                            gfx.RotateTransform(270);
                            break;
                        default:
                            gfx.TranslateTransform(dx, dy);
                            break;
                    }

                    gfx.DrawImage(form, new XRect(0, 0, scaledWidth, scaledHeight));
                    gfx.Restore();
                }

                for (var pageIndex = 0; pageIndex < form.PageCount; pageIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Select the source page
                    form.PageNumber = pageIndex + 1;

                    // Determine source page sizes
                    var srcWidth = form.PointWidth;
                    var srcHeight = form.PointHeight;

                    // Read low-level PDF page info to get rotation
                    var pdfPage = inputDoc.Pages[pageIndex];
                    int rotate = 0;
                    if (pdfPage.Elements.ContainsKey("/Rotate"))
                        rotate = pdfPage.Elements.GetInteger("/Rotate");

                    // Log rotation only (per user's request)
                    logInfo += $"Page {pageIndex + 1}: Rotate={rotate}\n";

                    // If page rotation is 90 or 270, swap width/height for layout calculations
                    var effectiveSrcWidth = (rotate == 90 || rotate == 270) ? srcHeight : srcWidth;
                    var effectiveSrcHeight = (rotate == 90 || rotate == 270) ? srcWidth : srcHeight;

                    var landscape = effectiveSrcWidth > effectiveSrcHeight;

                    // Read low-level PDF page info to get rotation
                    var pdfPage = inputDoc.Pages[pageIndex];
                    int rotate = 0;
                    if (pdfPage.Elements.ContainsKey("/Rotate"))
                        rotate = pdfPage.Elements.GetInteger("/Rotate");

                    // If the imported form or the low-level page has a rotation, zero it so we control drawing
                    if (pdfPage.Elements.ContainsKey("/Rotate"))
                        pdfPage.Elements["/Rotate"] = new PdfInteger(0);
                    if (form.Page != null && form.Page.Elements.ContainsKey("/Rotate"))
                        form.Page.Elements["/Rotate"] = new PdfInteger(0);

                    // If page rotation is 90 or 270, swap width/height for layout calculations
                    bool pageIsRotated = rotate == 90 || rotate == 270;
                    var visibleSrcWidth = pageIsRotated ? srcHeight : srcWidth;
                    var visibleSrcHeight = pageIsRotated ? srcWidth : srcHeight;
                    var visibleLandscape = visibleSrcWidth > visibleSrcHeight;

                    // Determine target dimensions based on visible orientation
                    var targetWidth = visibleLandscape ? targetSizeHeightPt : targetSizeWidthPt;
                    var targetHeight = visibleLandscape ? targetSizeWidthPt : targetSizeHeightPt;

                    // This is needed to avoid scaling up pages that are smaller than the target size when OnlyScaleDown is true
                    bool pageIsSmallerThanTarget = effectiveSrcWidth <= targetWidth && effectiveSrcHeight <= targetHeight;

                    if (input.OnlyScaleDown && pageIsSmallerThanTarget)
                    {
                        // Copy the page exactly as is without scaling (centered)
                        newPage.Width = XUnit.FromPoint(targetWidth);
                        newPage.Height = XUnit.FromPoint(targetHeight);
                        using var gfx = XGraphics.FromPdfPage(newPage);

                        var drawScale = 1.0;
                        var visibleDrawWidth = visibleSrcWidth * drawScale;
                        var visibleDrawHeight = visibleSrcHeight * drawScale;
                        var dx = (targetWidth - visibleDrawWidth) / 2.0;
                        var dy = (targetHeight - visibleDrawHeight) / 2.0;

                        DrawRotatedForm(gfx, form, rotate, drawScale, dx, dy);
                        logInfo += $"Page {pageIndex + 1}: no scale draw at dx={dx}, dy={dy}, drawScale={drawScale}\n";
                    }
                    else
                    {
                        // Scale the page to fit inside the target size
                        newPage.Width = XUnit.FromPoint(targetWidth);
                        newPage.Height = XUnit.FromPoint(targetHeight);
                        using var gfx = XGraphics.FromPdfPage(newPage);

                        // Compute scale to fit inside the target dimensions while maintaining aspect ratio
                        var scaleX = targetWidth / effectiveSrcWidth;
                        var scaleY = targetHeight / effectiveSrcHeight;
                        var scale = Math.Min(scaleX, scaleY);
                        // Center the scaled content
                        var drawWidth = effectiveSrcWidth * scale;
                        var drawHeight = effectiveSrcHeight * scale;
                        var dx = (targetWidth - drawWidth) / 2.0;
                        var dy = (targetHeight - drawHeight) / 2.0;

                        DrawRotatedForm(gfx, form, rotate, scale, dx, dy);
                        logInfo += $"Page {pageIndex + 1}: scaleX={scaleX}, scaleY={scaleY}, chosenScale={scale}, dx={dx}, dy={dy}\n";
                    }
                }

                using var outputStream = new MemoryStream();
                output.Save(outputStream);

                string resultBase64 = Convert.ToBase64String(outputStream.ToArray());

                return new Result { Success = true, ResultBase64 = resultBase64, Error = null, Info = logInfo };
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                return Helpers.ErrorHandler.Handle(e, options.ThrowErrorOnFailure, options.ErrorMessageOnFailure);
            }
        }
    }
}