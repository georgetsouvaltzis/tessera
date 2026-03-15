using TeaSharp.Components.Composition;

namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Provides discoverable factory methods for the stable prebuilt widget surface.
/// </summary>
internal static class PrebuiltCatalog
{

    public static LayoutContainerComponent LayoutContainer(LayoutContainerOptions? options = null) => options is null ? new LayoutContainerComponent() : new LayoutContainerComponent(options);
}
