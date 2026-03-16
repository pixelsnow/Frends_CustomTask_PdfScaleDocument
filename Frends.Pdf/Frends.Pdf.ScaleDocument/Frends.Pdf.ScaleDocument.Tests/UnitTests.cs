using System;
using System.IO;
using System.Threading;
using Frends.Pdf.ScaleDocument.Definitions;
using MigraDoc.DocumentObjectModel;
using NUnit.Framework;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Frends.Pdf.ScaleDocument.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string TestDataDir = Path.Combine(AppContext.BaseDirectory, "TestData");
    private static readonly string ExpectedOutputPortraitPath = Path.Combine(TestDataDir, "A4_portrait_2pages.pdf");
    private static readonly string ExpectedOutputLandscapePath = Path.Combine(TestDataDir, "A4_landscape_2pages.pdf");

    private static Input PortraitInput => new()
    {
        InputBase64 = ReadFileAsBase64(Path.Combine(TestDataDir, "A3_portrait_2pages.pdf")),
        Size = PageSizeEnum.A4,
    };

    private static Input LandscapeInput => new()
    {
        InputBase64 = ReadFileAsBase64(Path.Combine(TestDataDir, "A3_landscape_2pages.pdf")),
        Size = PageSizeEnum.A4,
    };

    private static Input InvalidBase64Input => new()
    {
        InputBase64 = "invalid_base64_string",
        Size = PageSizeEnum.A4,
    };

    private static Input InvalidNonPdfInput => new()
    {
        InputBase64 = ReadFileAsBase64(Path.Combine(TestDataDir, "invalid.txt")),
        Size = PageSizeEnum.A4,
    };

    private static Options DefaultOptions => new()
    {
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
        FileExistsAction = FileExistsActionEnum.Error,
    };

    [Test]
    public void ShouldScalePortraitFile()
    {
        var input = PortraitInput;
        var result = Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(BinaryPdfsHaveSamePageDimensions(File.ReadAllBytes(ExpectedOutputPortraitPath), Convert.FromBase64String(result.ResultFilePath)), Is.True);
    }

    [Test]
    public void ShouldScaleLandscapeFile()
    {
        var input = LandscapeInput;
        var result = Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None);
        Assert.That(result.Success, Is.True);
        Assert.That(BinaryPdfsHaveSamePageDimensions(File.ReadAllBytes(ExpectedOutputLandscapePath), Convert.FromBase64String(result.ResultFilePath)), Is.True);
    }

    [Test]
    public void ShouldFailIfbase64StringIsInvalid()
    {
        var input = InvalidBase64Input;
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ShouldFailIfInputIsNotPdfEvenIfBase64()
    {
        var input = InvalidNonPdfInput;
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    /*
    [Test]
    public void ShouldFailIfOutputPathAlreadyOccupied()
    {
        Pdf.ScaleDocument(DefaultInput, DefaultOptions, CancellationToken.None);
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(DefaultInput, DefaultOptions, CancellationToken.None));
    }
    */

    [Test]
    public void ReturnFailedResultIfThrowErrorFlagIsDisabled()
    {
        var input = InvalidNonPdfInput;
        var options = DefaultOptions;
        options.ThrowErrorOnFailure = false;
        var result = Pdf.ScaleDocument(input, options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void DefaultErrorMessageOnFailureIsUsed()
    {
        var input = InvalidNonPdfInput;
        var options = DefaultOptions;
        options.ErrorMessageOnFailure = "Test message";
        var ex = Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, options, CancellationToken.None));
        Assert.That(ex!.Message, Is.EqualTo("Test message"));
    }

    private static bool BinaryPdfsHaveSamePageDimensions(byte[] path1, byte[] path2)
    {
        using var stream1 = new MemoryStream(path1);
        using var stream2 = new MemoryStream(path2);
        using var doc1 = PdfReader.Open(stream1, PdfDocumentOpenMode.Import);
        using var doc2 = PdfReader.Open(stream2, PdfDocumentOpenMode.Import);

        // First make sure that files have the same number of pages
        if (doc1.PageCount != doc2.PageCount)
            return false;

        // Iterate through pages and make sure dimensions are identical
        for (int i = 0; i < doc1.PageCount; i++)
        {
            // We don't need sizes to be exactly equal, let's forgive some small tolerance for rounding differences
            if (Math.Abs(doc1.Pages[i].Width.Point - doc2.Pages[i].Width.Point) > 0.01 ||
                Math.Abs(doc1.Pages[i].Height.Point - doc2.Pages[i].Height.Point) > 0.01)
                return false;
        }

        return true;
    }

    private static string ReadFileAsBase64(string path) => Convert.ToBase64String(File.ReadAllBytes(path));
}