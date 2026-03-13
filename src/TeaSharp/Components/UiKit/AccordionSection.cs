using System.ComponentModel;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
namespace TeaSharp.Components.UiKit;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct AccordionSection(string Title, IReadOnlyList<string> BodyLines, bool Expanded = false);
