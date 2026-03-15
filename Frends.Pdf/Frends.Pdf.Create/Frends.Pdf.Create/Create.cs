using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Frends.Pdf.Create.Definitions;
using Frends.Pdf.Create.Helpers;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace Frends.Pdf.Create;

/// <summary>
/// Task class.
/// </summary>
public static class Pdf
{
    /// <summary>
    /// Create PDF document from given content.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.PDF.Create)
    /// Requirements:
    /// To use the task in Linux agent, you need to install packages libgdiplus, apt-utils and libc6-dev,
    /// since the task uses Windows-based graphics to draw elements to the PDF-file. These packages will
    /// emulate Windows based graphics in Linux. Installing those packages is only availably on on-premises agent.
    /// </summary>
    /// <param name="outputFile"></param>
    /// <param name="documentSettings"></param>
    /// <param name="content"></param>
    /// <param name="options"></param>
    /// <returns>Object { bool Success, string FileName }</returns>
    public static Result Create(
        [PropertyTab] FileProperties outputFile,
        [PropertyTab] DocumentSettings documentSettings,
        [PropertyTab] DocumentContent content,
        [PropertyTab] Options options
    )
    {
        try
        {
            GlobalFontSettings.FontResolver = new FileFontResolver();
            FileFontResolver.Setup(options.FallbackFontName, options.CustomFontsLocation);
            var document = new Document();
            if (!string.IsNullOrWhiteSpace(documentSettings.Title)) document.Info.Title = documentSettings.Title;
            if (!string.IsNullOrWhiteSpace(documentSettings.Author)) document.Info.Author = documentSettings.Author;

            AddContent(document, documentSettings, content);

            var fileName = DetermineFileName(outputFile);

            // Save document.
            var pdfRenderer = new PdfDocumentRenderer
            {
                Document = document,
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.Save(fileName);

            return new Result(true, fileName);
        }
        catch
        {
            if (options.ThrowErrorOnFailure) throw;

            return new Result(false, null);
        }
    }

    #region HelperMethods

    private static void AddContent(Document document, DocumentSettings documentSettings, DocumentContent content)
    {
        // Get the selected page size.
        PageSetup.GetPageSize(documentSettings.Size.ConvertEnum<PageFormat>(), out Unit width, out Unit height);
        var section = document.AddSection();
        SetupPage(section.PageSetup, width, height, documentSettings);

        // Index for stylename.
        var elementNumber = 0;

        // Add page elements.
        foreach (var pageElement in content.Contents)
        {
            var styleName = $"style_{elementNumber}";
            var style = document.Styles.AddStyle(styleName, "Normal");

            switch (pageElement.ContentType)
            {
                case ElementType.Image:
                    AddImage(section, pageElement, width);

                    break;
                case ElementType.PageBreak:
                    section = document.AddSection();
                    SetupPage(section.PageSetup, width, height, documentSettings);

                    break;
                case ElementType.Header:
                    SetFont(style, pageElement);
                    SetParagraphStyle(style, pageElement);
                    AddHeaderFooterContent(section, pageElement, style, true);

                    break;
                case ElementType.Footer:
                    SetFont(style, pageElement);
                    SetParagraphStyle(style, pageElement);
                    AddHeaderFooterContent(section, pageElement, style, false);

                    break;
                case ElementType.Table:
                    AddTable(section, pageElement, width);

                    break;
                default:
                    SetFont(style, pageElement);
                    SetParagraphStyle(style, pageElement);
                    AddTextContent(section, pageElement, style);

                    break;
            }

            elementNumber++;
        }
    }

    private static string DetermineFileName(FileProperties outputFile)
    {
        var fileName = Path.Combine(outputFile.Directory, outputFile.FileName);
        var fileNameIndex = 1;

        if (File.Exists(fileName) && outputFile.FileExistsAction == FileExistsActionEnum.Error)
            throw new Exception("Output file already exists: " + fileName);

        while (File.Exists(fileName) && outputFile.FileExistsAction != FileExistsActionEnum.Overwrite)
        {
            switch (outputFile.FileExistsAction)
            {
                case FileExistsActionEnum.Error:
                    throw new Exception($"File {fileName} already exists.");
                case FileExistsActionEnum.Rename:
                    fileName = Path.Combine(outputFile.Directory,
                        $"{Path.GetFileNameWithoutExtension(outputFile.FileName)}_({fileNameIndex}){Path.GetExtension(outputFile.FileName)}");

                    break;
            }

            fileNameIndex++;
        }

        return fileName;
    }

    private static void SetupPage(PageSetup setup, Unit pageWidth, Unit pageHeight, DocumentSettings documentSettings)
    {
        setup.Orientation = documentSettings.Orientation.ConvertEnum<Orientation>();
        setup.PageHeight = pageHeight;
        setup.PageWidth = pageWidth;
        setup.LeftMargin = new Unit(documentSettings.MarginLeftInCm, UnitType.Centimeter);
        setup.TopMargin = new Unit(documentSettings.MarginTopInCm, UnitType.Centimeter);
        setup.RightMargin = new Unit(documentSettings.MarginRightInCm, UnitType.Centimeter);
        setup.BottomMargin = new Unit(documentSettings.MarginBottomInCm, UnitType.Centimeter);
    }

    private static void SetParagraphStyle(Style style, PageContentElement pageContent)
    {
        style.ParagraphFormat.LineSpacing = new Unit(pageContent.LineSpacingInPt, UnitType.Point);
        style.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
        style.ParagraphFormat.Alignment =
            pageContent.ParagraphAlignment.ConvertEnum<ParagraphAlignment>();
        style.ParagraphFormat.SpaceBefore = new Unit(pageContent.SpacingBeforeInPt, UnitType.Point);
        style.ParagraphFormat.SpaceAfter = new Unit(pageContent.SpacingAfterInPt, UnitType.Point);
    }

    private static void AddImage(Section section, PageContentElement pageContent, Unit pageWidth)
    {
        if (!File.Exists(pageContent.ImagePath))
            throw new FileNotFoundException("Image not found from path: " + pageContent.ImagePath);

        using var xImage = XImage.FromFile(pageContent.ImagePath);
        var originalImageWidthInches = Unit.FromPoint(xImage.PointWidth);


        // Add image.
        var image = section.AddImage(pageContent.ImagePath);

        // Calculate Image size.
        // If actual image size is larger than PageWidth - margins, set image width as page width - margins.
        var actualPageContentWidth =
            new Unit((pageWidth.Inch - section.PageSetup.LeftMargin.Inch - section.PageSetup.RightMargin.Inch),
                UnitType.Inch);
        if (originalImageWidthInches > actualPageContentWidth) image.Width = actualPageContentWidth;
        image.LockAspectRatio = true;
        image.Left = pageContent.ImageAlignment.ConvertEnum<ShapePosition>();
    }

    private static void AddTextContent(Section section, PageContentElement pageContent, Style style)
    {
        // Skip if text content is empty.
        if (string.IsNullOrWhiteSpace(pageContent.Text)) return;

        var paragraph = section.AddParagraph();
        paragraph.Style = style.Name;
        paragraph.Format.Font.Color = Colors.Black;

        // Read text line by line.
        using var reader = new StringReader(pageContent.Text);

        while (reader.ReadLine() is { } line)
        {
            // Read text one char at a time, so that multiple whitespaces are added correctly.
            foreach (var character in line.ToCharArray())
            {
                if (char.IsWhiteSpace(character)) paragraph.AddSpace(1);
                else paragraph.AddChar(character, 1);
            }

            // Add newline.
            paragraph.AddLineBreak();
        }
    }

    private static void AddHeaderFooterContent(Section section, PageContentElement pageContent, Style style,
        bool isHeader)
    {
        // Skip if text content is empty.
        if (string.IsNullOrWhiteSpace(pageContent.Text)) return;

        Table table;

        if (isHeader)
            table = section.Headers.Primary.AddTable();
        else
            table = section.Footers.Primary.AddTable();

        Row row;
        Paragraph textField;
        Paragraph pagenumField;

        switch (pageContent.HeaderFooterStyle)
        {
            case HeaderFooterStyleEnum.Text:
                table.AddColumn("16cm");
                row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                textField = row.Cells[0].AddParagraph();

                break;
            case HeaderFooterStyleEnum.TextPagenum:
                table.AddColumn("12cm");
                table.AddColumn("4cm");
                row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                textField = row.Cells[0].AddParagraph();
                pagenumField = row.Cells[1].AddParagraph();
                FormatPagenumField(style, pagenumField);

                break;
            case HeaderFooterStyleEnum.LogoText:
                table.AddColumn("5cm");
                table.AddColumn("11cm");
                row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                FormatHeaderFooterLogo(pageContent, row);
                textField = row.Cells[1].AddParagraph();

                break;
            case HeaderFooterStyleEnum.LogoTextPagenum:
                table.AddColumn("5cm");
                table.AddColumn("7cm");
                table.AddColumn("4cm");
                row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                FormatHeaderFooterLogo(pageContent, row);
                textField = row.Cells[1].AddParagraph();
                pagenumField = row.Cells[2].AddParagraph();
                FormatPagenumField(style, pagenumField);

                break;
            default:
                throw new Exception($"Cannot insert header without proper style choice.");
        }

        textField.Style = style.Name;
        textField.Format.Font.Color = Colors.Black;
        textField.AddText(pageContent.Text);

        if (pageContent.BorderWidthInPt > 0 && isHeader)
            table.Borders.Bottom.Width = new Unit(pageContent.BorderWidthInPt, UnitType.Point);
        else if (pageContent.BorderWidthInPt > 0 && !isHeader)
            table.Borders.Top.Width = new Unit(pageContent.BorderWidthInPt, UnitType.Point);
    }

    private static void AddTable(Section section, PageContentElement pageContent, Unit pageWidth)
    {
        TableDefinition tableData = JsonConvert.DeserializeObject<TableDefinition>(pageContent.Table);
        Table table;

        switch (tableData.TableType)
        {
            case TableTypeEnum.Header:
                table = section.Headers.Primary.AddTable();

                break;
            case TableTypeEnum.Footer:
                table = section.Footers.Primary.AddTable();

                break;
            default:
                table = section.AddTable();

                break;
        }

        var tableWidth = new Unit(0, UnitType.Centimeter);
        var actualPageContentWidth =
            new Unit(
                (pageWidth.Centimeter - section.PageSetup.LeftMargin.Centimeter -
                 section.PageSetup.RightMargin.Centimeter), UnitType.Centimeter);

        foreach (var column in tableData.Columns)
        {
            var columnWidth = new Unit(column.WidthInCm, UnitType.Centimeter);
            tableWidth += columnWidth;

            if (tableWidth > actualPageContentWidth)
                throw new Exception(
                    $"Page allows table to be {actualPageContentWidth.Centimeter} cm wide. Provided table's width is larger than that, {tableWidth.Centimeter} cm.");
            table.AddColumn(columnWidth);
        }

        if (tableData.HasHeaderRow)
        {
            var columnHeaders = tableData.Columns.Select(column => column.Name).ToList();
            var headerColumnDefinitions = new List<TableColumnDefinition>();
            for (int i = 0; i < columnHeaders.Count; i++)
                headerColumnDefinitions.Add(new TableColumnDefinition
                {
                    Type = TableColumnType.Text
                });
            ProcessRow(table, headerColumnDefinitions, columnHeaders, tableData.StyleSettings);
        }

        foreach (var dataRow in tableData.RowData)
        {
            var data = dataRow.Select(row => row.Value).ToList();
            ProcessRow(table, tableData.Columns, data, tableData.StyleSettings);
        }

        if (tableData.StyleSettings.BorderWidthInPt > 0)
        {
            switch (tableData.StyleSettings.BorderStyle)
            {
                case TableBorderStyle.Top:
                    table.Borders.Top.Width = new Unit(tableData.StyleSettings.BorderWidthInPt, UnitType.Point);

                    break;
                case TableBorderStyle.Bottom:
                    table.Borders.Bottom.Width = new Unit(tableData.StyleSettings.BorderWidthInPt, UnitType.Point);

                    break;
                case TableBorderStyle.All:
                    table.Borders.Width = new Unit(tableData.StyleSettings.BorderWidthInPt, UnitType.Point);

                    break;
                case TableBorderStyle.None:
                    break;
            }
        }
    }

    private static void ProcessRow(Table table, List<TableColumnDefinition> columns, List<string> data,
        TableStyle style)
    {
        var row = table.AddRow();
        row.VerticalAlignment = VerticalAlignment.Center;

        for (int i = 0; i < data.Count; i++)
        {
            switch (columns[i].Type)
            {
                case TableColumnType.Text:
                    var textField = row.Cells[i].AddParagraph();
                    SetParagraphStyle(textField, style);
                    textField.AddText(data[i]);

                    break;
                case TableColumnType.Image:
                    if (string.IsNullOrWhiteSpace(data[i]) || !File.Exists(data[i]))
                        throw new FileNotFoundException(
                            $"Path to header graphics was empty or the file does not exist.");
                    var logo = row.Cells[i].AddImage(data[i]);
                    logo.Height = new Unit(columns[i].HeightInCm, UnitType.Centimeter);
                    logo.LockAspectRatio = true;
                    logo.Top = ShapePosition.Top;
                    logo.Left = ShapePosition.Left;

                    break;
                case TableColumnType.PageNum:
                    var pagenumField = row.Cells[i].AddParagraph();
                    SetParagraphStyle(pagenumField, style);
                    pagenumField.AddPageField();
                    pagenumField.AddText(" (");
                    pagenumField.AddNumPagesField();
                    pagenumField.AddText(")");

                    break;
            }
        }
    }

    private static void SetParagraphStyle(Paragraph pg, TableStyle style)
    {
        pg.Format.Font.Color = Colors.Black;
        pg.Format.Font.Name = style.FontFamily;
        pg.Format.Font.Size = new Unit(style.FontSizeInPt, UnitType.Point);

        switch (style.FontStyle)
        {
            case FontStyleEnum.Bold:
                pg.Format.Font.Bold = true;

                break;
            case FontStyleEnum.Italic:
                pg.Format.Font.Italic = true;

                break;
            case FontStyleEnum.BoldItalic:
                pg.Format.Font.Bold = true;
                pg.Format.Font.Italic = true;

                break;
            case FontStyleEnum.Underline:
                pg.Format.Font.Underline = Underline.Single;

                break;
        }

        pg.Format.LineSpacing = new Unit(style.LineSpacingInPt, UnitType.Point);
        pg.Format.SpaceBefore = new Unit(style.SpacingBeforeInPt, UnitType.Point);
        pg.Format.SpaceAfter = new Unit(style.SpacingAfterInPt, UnitType.Point);
    }

    private static void FormatHeaderFooterLogo(PageContentElement pageContent, Row row)
    {
        if (string.IsNullOrWhiteSpace(pageContent.ImagePath) || !File.Exists(pageContent.ImagePath))
            throw new FileNotFoundException("Path to header graphics was empty or the file does not exist: " +
                                            pageContent.ImagePath);

        var logo = row.Cells[0].AddImage(pageContent.ImagePath);
        logo.Height = new Unit(pageContent.ImageHeightInCm, UnitType.Centimeter);
        logo.LockAspectRatio = true;
        logo.Top = ShapePosition.Top;
        logo.Left = ShapePosition.Left;
    }

    private static void FormatPagenumField(Style style, Paragraph pagenumField)
    {
        pagenumField.Style = style.Name;
        pagenumField.Format.Alignment = ParagraphAlignment.Right;
        pagenumField.Format.Font.Color = Colors.Black;
        pagenumField.AddPageField();
        pagenumField.AddText(" (");
        pagenumField.AddNumPagesField();
        pagenumField.AddText(")");
    }

    private static void SetFont(Style style, PageContentElement textElement)
    {
        style.Font.Name = textElement.FontFamily;
        style.Font.Size = new Unit(textElement.FontSize, UnitType.Point);

        switch (textElement.FontStyle)
        {
            case FontStyleEnum.Bold:
                style.Font.Bold = true;

                break;
            case FontStyleEnum.BoldItalic:
                style.Font.Bold = true;
                style.Font.Italic = true;

                break;
            case FontStyleEnum.Italic:
                style.Font.Italic = true;

                break;
            case FontStyleEnum.Underline:
                style.Font.Underline = Underline.Single;

                break;
        }
    }

    #endregion

}
