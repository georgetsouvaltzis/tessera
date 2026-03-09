using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record ListOptions<T>(
    IEnumerable<T> Items,
    Func<T, string> ToText,
    string Title = "List",
    bool Focused = false,
    bool Disabled = false,
    bool ReadOnly = false,
    bool ShowBorder = true,
    ListKeyMap? KeyMap = null);
