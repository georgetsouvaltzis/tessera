
namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Provides discoverable factory methods for the stable prebuilt widget surface.
/// </summary>
public static class PrebuiltCatalog
{
    public static LabelComponent Label(LabelOptions? options = null) => options is null ? new LabelComponent() : new LabelComponent(options);

    public static ButtonComponent Button(ButtonOptions? options = null) => options is null ? new ButtonComponent() : new ButtonComponent(options);

    public static TextInputComponent TextInput(TextInputOptions? options = null) => options is null ? new TextInputComponent() : new TextInputComponent(options);

    public static TextAreaComponent TextArea(TextAreaOptions? options = null) => options is null ? new TextAreaComponent() : new TextAreaComponent(options);

    public static ListComponent<T> List<T>(IEnumerable<T> items, Func<T, string> toText) => new(items, toText);

    public static ListComponent<T> List<T>(ListOptions<T> options) => new(options);

    public static TableComponent Table(IReadOnlyList<string> headers) => new(headers);

    public static TableComponent Table(TableOptions options) => new(options);

    public static DropdownComponent Dropdown(DropdownOptions? options = null) => options is null ? new DropdownComponent() : new DropdownComponent(options);

    public static ComboboxComponent Combobox(ComboboxOptions? options = null) => options is null ? new ComboboxComponent() : new ComboboxComponent(options);

    public static ProgressBarComponent ProgressBar(ProgressBarOptions? options = null) => options is null ? new ProgressBarComponent() : new ProgressBarComponent(options);

    public static StatusBarComponent StatusBar(StatusBarOptions? options = null) => options is null ? new StatusBarComponent() : new StatusBarComponent(options);

    public static LogViewerComponent LogViewer(LogViewerOptions? options = null) => options is null ? new LogViewerComponent() : new LogViewerComponent(options);

    public static DialogComponent Dialog(DialogOptions? options = null) => options is null ? new DialogComponent() : new DialogComponent(options);

    public static LayoutContainerComponent LayoutContainer(LayoutContainerOptions? options = null) => options is null ? new LayoutContainerComponent() : new LayoutContainerComponent(options);
}
