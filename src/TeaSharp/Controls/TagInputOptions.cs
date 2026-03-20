using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Defines advanced behavior options for <see cref="TagInput"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct TagInputOptions(
    char Separator = ',',
    bool AllowDuplicates = false,
    bool CaseSensitive = false,
    int MaxTags = 0,
    bool ShowTagCount = false,
    string TagPrefix = "[",
    string TagSuffix = "]");
