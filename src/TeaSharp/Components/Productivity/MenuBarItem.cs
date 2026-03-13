using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;
using System.ComponentModel;

namespace TeaSharp.Components.Productivity;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed record MenuBarItem(
    string Id,
    string Title,
    char Shortcut = '\0',
    IReadOnlyCollection<WidgetVisualState>? States = null);
