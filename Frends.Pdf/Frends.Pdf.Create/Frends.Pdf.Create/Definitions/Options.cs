using System.ComponentModel;

namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Class for task options.
/// </summary>
public class Options
{
    /// <summary>
    /// True: Throws error on failure
    /// False: Returns object{ Success = false }
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Path to directory with fonts to use.
    /// If empty, task will use default system locations
    /// Otherwise, CustomFontsLocation will be used as well as default system locations, unless it does not exist.
    /// </summary>
    /// <example>C:\MyDir\fonts</example>
    [DefaultValue("")]
    public string CustomFontsLocation { get; set; } = string.Empty;

    /// <summary>
    /// Font family name that will be used, if specific font couldn't be resolved.
    /// Using Arial if nothing is provided.
    /// </summary>
    /// <example>Arial</example>
    [DefaultValue("")]
    public string FallbackFontName { get; set; } = string.Empty;
}
