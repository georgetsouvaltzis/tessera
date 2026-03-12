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
public enum TimePickerField
{
    Hour = 0,
    Minute = 1,
    Second = 2,
}
