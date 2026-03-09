using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record MenuBarItem(
    string Id,
    string Title,
    char Shortcut = '\0',
    IReadOnlyCollection<WidgetVisualState>? States = null);

