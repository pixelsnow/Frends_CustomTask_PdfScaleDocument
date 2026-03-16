using System;
using System.Diagnostics; // TODO: remove after debugging
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
    private static readonly string ResultDir = Path.Combine(TestDataDir, "result");
    private static readonly string ExpectedOutputPath = Path.Combine(TestDataDir, "expectedOutput.pdf");
    private static readonly string ExpectedOutputPortraitPath = Path.Combine(TestDataDir, "A4_portrait_2pages.pdf");
    private static readonly string ExpectedOutputLandscapePath = Path.Combine(TestDataDir, "A4_landscape_2pages.pdf");

    private static Input PortraitInput => new()
    {
        InputFilePath = Path.Combine(TestDataDir, "A3_portrait_2pages.pdf"),
        DestinationFilePath = Path.Combine(ResultDir, "A4_portrait_2pages.pdf"),
        Size = PageSizeEnum.A4,
    };

    private static Input LandscapeInput => new()
    {
        InputFilePath = Path.Combine(TestDataDir, "A3_landscape_2pages.pdf"),
        DestinationFilePath = Path.Combine(ResultDir, "A4_landscape_2pages.pdf"),
        Size = PageSizeEnum.A4,
    };

    private static Options DefaultOptions => new()
    {
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
        FileExistsAction = FileExistsActionEnum.Error,
    };

    [SetUp]
    public void ClearResults()
    {
        if (Path.Exists(ResultDir))
        {
            if (File.Exists(ResultDir)) File.Delete(ResultDir);
            else
                Directory.Delete(ResultDir, recursive: true);
        }
    }

    [Test]
    public void ShouldScalePortraitFile()
    {
        var input = PortraitInput;
        var result = Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None);
        Assert.That(FilesHaveSameDimensions(ExpectedOutputPortraitPath, input.DestinationFilePath), Is.True);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ShouldScaleLandscapeFile()
    {
        var input = LandscapeInput;
        var result = Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None);
        Assert.That(FilesHaveSameDimensions(ExpectedOutputLandscapePath, input.DestinationFilePath), Is.True);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ShouldFailIfInputFileDoesNotExist()
    {
        var input = PortraitInput;
        input.InputFilePath = Path.Combine(TestDataDir, "nonexistent.pdf");
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ShouldFailIfInputFileIsNotPdf()
    {
        var input = PortraitInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
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
    public void ShouldFailIfOutputPathIsInvalid()
    {
        var input = PortraitInput;
        input.DestinationFilePath = Path.Combine(TestDataDir, "result");
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ReturnFailedResultIfThrowErrorFlagIsDisabled()
    {
        var input = PortraitInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
        var options = DefaultOptions;
        options.ThrowErrorOnFailure = false;
        var result = Pdf.ScaleDocument(input, options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void DefaultErrorMessageOnFailureIsUsed()
    {
        var input = PortraitInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
        var options = DefaultOptions;
        options.ErrorMessageOnFailure = "Test message";
        var ex = Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, options, CancellationToken.None));
        Assert.That(ex!.Message, Is.EqualTo("Test message"));
    }

    /*
    private static bool FilesHaveSameSize(string path1, string path2)
    {
        TestContext.WriteLine($"Comparing file sizes: {path1} and {path2}");
        var f1 = new FileInfo(path1);
        var f2 = new FileInfo(path2);
        TestContext.WriteLine($"File sizes: {f1.Length} and {f2.Length}");
        return f1.Length == f2.Length;
    }
    */

    private static bool FilesHaveSameDimensions(string path1, string path2)
    {
        using var doc1 = PdfReader.Open(path1, PdfDocumentOpenMode.Import);
        using var doc2 = PdfReader.Open(path2, PdfDocumentOpenMode.Import);

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
}