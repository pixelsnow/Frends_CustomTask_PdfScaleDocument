using System;
using System.IO;
using System.Threading;
using Frends.Pdf.Read.Definitions;
using Frends.Pdf.Read.DTOs;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Frends.Pdf.Read.Tests;

[TestFixture]
public class UnitTests
{
    private string testDataFolder;
    private Options options;

    [SetUp]
    public void Setup()
    {
        testDataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../TestData");

        options = new Options
        {
            ThrowErrorOnFailure = true,
        };
    }

    [Test]
    public void ReadPdf_AllPages_ReturnsPagesWithMetadata()
    {
        var result = Pdf.Read(
            new Input
            {
                FilePath = Path.Combine(testDataFolder, "multi-page.pdf"),
            },
            options,
            CancellationToken.None);

        Assert.That(result.Success, Is.True);

        var document = JsonConvert.DeserializeObject<PdfReadDocument>(result.Json);

        Assert.That(result.Success, Is.True);
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Pages, Is.Not.Null);
        Assert.That(document.Pages.Count, Is.GreaterThan(1));
        Assert.That(document.Metadata.Author, Is.Not.Null);
        Assert.That(document.Metadata.Title, Is.Not.Null);
    }

    [Test]
    public void ReadPdf_SinglePagePdf_ReturnsOnePageWithMetadata()
    {
        var result = Pdf.Read(
            new Input
            {
                FilePath = Path.Combine(testDataFolder, "single-page.pdf"),
            },
            options,
            CancellationToken.None);

        var document = JsonConvert.DeserializeObject<PdfReadDocument>(result.Json);

        Assert.That(result.Success, Is.True);
        Assert.That(document.Pages.Count, Is.EqualTo(1));
        Assert.That(string.IsNullOrWhiteSpace(document.Pages[0].Text), Is.False);
        Assert.That(document.Metadata.Author, Is.Not.Null);
        Assert.That(document.Metadata.Title, Is.Not.Null);
    }

    [Test]
    public void ReadPdf_IncludeImages()
    {
        var result = Pdf.Read(
            new Input
            {
                FilePath = Path.Combine(testDataFolder, "Png_image.pdf"),
            },
            new Options
            {
                IncludeImages = true,
            },
            CancellationToken.None);

        var document = JsonConvert.DeserializeObject<PdfReadDocument>(result.Json);
        Assert.That(result.Success, Is.True);
        Assert.That(document.Pages[0].Images.Count, Is.GreaterThan(0));
        Assert.That(document.Pages[0].Images[0].Bytes.Length, Is.GreaterThan(0));
    }
}
