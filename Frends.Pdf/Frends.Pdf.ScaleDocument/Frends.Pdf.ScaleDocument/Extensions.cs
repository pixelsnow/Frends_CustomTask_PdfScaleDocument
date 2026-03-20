using System;

namespace Frends.Pdf.ScaleDocument;

// Extension class for extending class methods.
public static class Extensions
{
    // Extension method to make enum conversion.
    public static TEnum ConvertEnum<TEnum>(this Enum source)
    {
        return (TEnum)Enum.Parse(typeof(TEnum), source.ToString(), true);
    }
}
