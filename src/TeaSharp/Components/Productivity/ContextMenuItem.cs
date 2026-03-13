using System.ComponentModel;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record ContextMenuItem(
    string Id,
    string Title,
    IReadOnlyCollection<WidgetVisualState>? States = null);
