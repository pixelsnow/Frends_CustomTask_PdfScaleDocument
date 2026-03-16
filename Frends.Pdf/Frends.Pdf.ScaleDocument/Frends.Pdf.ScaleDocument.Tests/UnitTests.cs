using System;
using System.IO;
using System.Threading;
using Frends.Pdf.ScaleDocument.Definitions;
using NUnit.Framework;

namespace Frends.Pdf.ScaleDocument.Tests;

[TestFixture]
public class UnitTests
{
    private static readonly string TestDataDir = Path.Combine(AppContext.BaseDirectory, "TestData");
    private static readonly string ResultDir = Path.Combine(TestDataDir, "result");
    private static readonly string ExpectedOutputPath = Path.Combine(TestDataDir, "expectedOutput.pdf");

    private static Input DefaultInput => new()
    {
        InputFilePath = Path.Combine(TestDataDir, "third.pdf"),
        DestinationFilePath = Path.Combine(ResultDir, "output.pdf"),
    };

    private static Options DefaultOptions => new()
    {
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
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
    public void ShouldScaleFile()
    {
        var input = DefaultInput;
        var result = Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None);
        Assert.That(FilesHaveSameSize(ExpectedOutputPath, input.DestinationFilePath), Is.True);
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public void ShouldFailIfAnyInputFilesDoesNotExist()
    {
        var input = DefaultInput;
        input.InputFilePath = Path.Combine(TestDataDir, "nonexistent.pdf");
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ShouldFailIfAnyInputFilesIsNotPdf()
    {
        var input = DefaultInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ShouldFailIfOutputPathAlreadyOccupied()
    {
        Pdf.ScaleDocument(DefaultInput, DefaultOptions, CancellationToken.None);
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(DefaultInput, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ShouldFailIfOutputPathIsInvalid()
    {
        var input = DefaultInput;
        input.DestinationFilePath = Path.Combine(TestDataDir, "result");
        Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, DefaultOptions, CancellationToken.None));
    }

    [Test]
    public void ReturnFailedResultIfThrowErrorFlagIsDisabled()
    {
        var input = DefaultInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
        var options = DefaultOptions;
        options.ThrowErrorOnFailure = false;
        var result = Pdf.ScaleDocument(input, options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void DefaultErrorMessageOnFailureIsUsed()
    {
        var input = DefaultInput;
        input.InputFilePath = Path.Combine(TestDataDir, "invalid.txt");
        var options = DefaultOptions;
        options.ErrorMessageOnFailure = "Test message";
        var ex = Assert.Throws<Exception>(() => Pdf.ScaleDocument(input, options, CancellationToken.None));
        Assert.That(ex!.Message, Is.EqualTo("Test message"));
    }

    private static bool FilesHaveSameSize(string path1, string path2)
    {
        var f1 = new FileInfo(path1);
        var f2 = new FileInfo(path2);
        return f1.Length == f2.Length;
    }
}