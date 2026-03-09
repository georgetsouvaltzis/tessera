using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record CommandPaletteItem(
    string Id,
    string Title,
    string Description = "",
    IReadOnlyCollection<WidgetVisualState>? States = null);

