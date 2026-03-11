using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;

namespace TeaSharp.Components.UiKit;

/// <summary>
/// Provides discoverable factory methods for the stable UI-kit component surface.
/// </summary>
public static class UiKitCatalog
{
    public static TabsComponent Tabs(IEnumerable<string> tabs) => new(tabs);

    public static TabsComponent Tabs(TabsOptions options) => new(options);

    public static ModalComponent Modal(ModalOptions? options = null) => options is null ? new ModalComponent() : new ModalComponent(options);
}
