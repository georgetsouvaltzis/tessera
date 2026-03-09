using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record ContextMenuItem(
    string Id,
    string Title,
    IReadOnlyCollection<WidgetVisualState>? States = null);

