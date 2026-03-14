using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Provides discoverable factory methods for the stable productivity widget surface.
/// </summary>
internal static class ProductivityCatalog
{
    public static NumberInputComponent NumberInput(NumberInputOptions? options = null) => options is null ? new NumberInputComponent() : new NumberInputComponent(options);

    public static DatePickerComponent DatePicker(DatePickerOptions? options = null) => options is null ? new DatePickerComponent() : new DatePickerComponent(options);

    public static TimePickerComponent TimePicker(TimePickerOptions? options = null) => options is null ? new TimePickerComponent() : new TimePickerComponent(options);

    public static MarkdownViewerComponent MarkdownViewer(MarkdownViewerOptions? options = null) => options is null ? new MarkdownViewerComponent() : new MarkdownViewerComponent(options);
}
