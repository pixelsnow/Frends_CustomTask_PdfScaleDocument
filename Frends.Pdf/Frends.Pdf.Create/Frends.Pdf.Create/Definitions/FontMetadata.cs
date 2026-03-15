using System.IO;
using OpenFontSharp;

namespace Frends.Pdf.Create.Definitions;

internal class FontMetadata
{
    public string Name { get; }
    public string FullPath { get; }
    public bool IsBold { get; }
    public bool IsItalic { get; }

    public FontMetadata(string fontFilePath)
    {
        using var fs = new FileStream(fontFilePath, FileMode.Open, FileAccess.Read);
        var reader = new OpenFontReader();
        var typeface = reader.Read(fs);

        var (bold, italic) = GetStyleFromFlags(typeface);

        Name = typeface.Name;
        FullPath = fontFilePath;
        IsBold = bold;
        IsItalic = italic;
    }

    private static (bool, bool) GetStyleFromFlags(Typeface tf)
    {
        bool italic = (tf.OS2Table.fsSelection & 0x01) != 0;
        bool bold = (tf.OS2Table.fsSelection & 0x20) != 0;
        return (bold, italic);
    }
}
