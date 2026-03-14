using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Provides discoverable factory methods for the stable prebuilt widget surface.
/// </summary>
internal static class PrebuiltCatalog
{

    public static TableComponent Table(IReadOnlyList<string> headers) => new(headers);

    public static TableComponent Table(TableOptions options) => new(options);

    public static ProgressBarComponent ProgressBar(ProgressBarOptions? options = null) => options is null ? new ProgressBarComponent() : new ProgressBarComponent(options);


    public static LogViewerComponent LogViewer(LogViewerOptions? options = null) => options is null ? new LogViewerComponent() : new LogViewerComponent(options);

    public static DialogComponent Dialog(DialogOptions? options = null) => options is null ? new DialogComponent() : new DialogComponent(options);

    public static LayoutContainerComponent LayoutContainer(LayoutContainerOptions? options = null) => options is null ? new LayoutContainerComponent() : new LayoutContainerComponent(options);
}
