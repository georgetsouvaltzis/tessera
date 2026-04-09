
namespace Tessera.Layout;

/// <summary>
/// Imperative builder for window-shaped default screens.
/// </summary>
public sealed class WindowBuilder
{
    private readonly WindowLayout _layout = new();

    /// <summary>
    /// Sets the gap between named window sections.
    /// </summary>
    public WindowBuilder Gap(int gap)
    {
        _layout.Gap = gap;
        return this;
    }

    /// <summary>
    /// Sets the inner window padding.
    /// </summary>
    public WindowBuilder Padding(Thickness padding)
    {
        _layout.Padding = padding;
        return this;
    }

    /// <summary>
    /// Sets uniform inner window padding.
    /// </summary>
    public WindowBuilder Padding(int all) => Padding(Thickness.All(all));

    /// <summary>
    /// Sets the top section to a fixed-height content block.
    /// </summary>
    public WindowBuilder Header(int height, LayoutNode content, Thickness margin = default)
    {
        _layout.Header = LayoutSlot.Fixed(content, height, margin);
        return this;
    }

    /// <summary>
    /// Builds the top section with an imperative content builder.
    /// </summary>
    public WindowBuilder Header(int height, Action<ContentBuilder> configure, Thickness margin = default) =>
        Header(height, BuildContent(configure), margin);

    /// <summary>
    /// Builds the top section as a horizontal row of layout slots.
    /// </summary>
    /// <param name="height">The fixed header height in rows.</param>
    /// <param name="configure">The callback that configures row items.</param>
    /// <param name="margin">Optional outer margin applied to the header slot.</param>
    /// <returns>The current builder instance for chaining.</returns>
    public WindowBuilder HeaderRow(int height, Action<StackBuilder> configure, Thickness margin = default) =>
        Header(height, StackBuilder.BuildRow(configure), margin);

    /// <summary>
    /// Sets the bottom section to a fixed-height content block.
    /// </summary>
    public WindowBuilder Footer(int height, LayoutNode content, Thickness margin = default)
    {
        _layout.Footer = LayoutSlot.Fixed(content, height, margin);
        return this;
    }

    /// <summary>
    /// Builds the bottom section with an imperative content builder.
    /// </summary>
    public WindowBuilder Footer(int height, Action<ContentBuilder> configure, Thickness margin = default) =>
        Footer(height, BuildContent(configure), margin);

    /// <summary>
    /// Sets the left section to a fixed-width content block.
    /// </summary>
    public WindowBuilder Left(int width, LayoutNode content, Thickness margin = default)
    {
        _layout.Left = LayoutSlot.Fixed(content, width, margin);
        return this;
    }

    /// <summary>
    /// Builds the left section with an imperative content builder.
    /// </summary>
    public WindowBuilder Left(int width, Action<ContentBuilder> configure, Thickness margin = default) =>
        Left(width, BuildContent(configure), margin);

    /// <summary>
    /// Builds the left section as a panel.
    /// </summary>
    public WindowBuilder Left(int width, Action<PanelBuilder> configure, Thickness margin = default) =>
        Left(width, BuildPanel(configure), margin);

    /// <summary>
    /// Sets the right section to a fixed-width content block.
    /// </summary>
    public WindowBuilder Right(int width, LayoutNode content, Thickness margin = default)
    {
        _layout.Right = LayoutSlot.Fixed(content, width, margin);
        return this;
    }

    /// <summary>
    /// Builds the right section with an imperative content builder.
    /// </summary>
    public WindowBuilder Right(int width, Action<ContentBuilder> configure, Thickness margin = default) =>
        Right(width, BuildContent(configure), margin);

    /// <summary>
    /// Builds the right section as a panel.
    /// </summary>
    public WindowBuilder Right(int width, Action<PanelBuilder> configure, Thickness margin = default) =>
        Right(width, BuildPanel(configure), margin);

    /// <summary>
    /// Sets the main body content.
    /// </summary>
    public WindowBuilder Body(LayoutNode content)
    {
        _layout.Body = content;
        return this;
    }

    /// <summary>
    /// Builds the main body content imperatively.
    /// </summary>
    public WindowBuilder Body(Action<ContentBuilder> configure) => Body(BuildContent(configure));

    /// <summary>
    /// Sets the overlay content.
    /// </summary>
    public WindowBuilder Overlay(LayoutNode content)
    {
        _layout.Overlay = content;
        return this;
    }

    /// <summary>
    /// Builds the overlay content imperatively.
    /// </summary>
    public WindowBuilder Overlay(Action<ContentBuilder> configure) => Overlay(BuildContent(configure));

    internal WindowLayout Build() => _layout;

    private static LayoutNode BuildContent(Action<ContentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new ContentBuilder();
        configure(builder);
        return builder.Build();
    }

    private static PanelLayout BuildPanel(Action<PanelBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new PanelBuilder();
        configure(builder);
        return builder.Build();
    }
}

/// <summary>
/// Imperative builder for nested layout content.
/// </summary>
public sealed class ContentBuilder
{
    private LayoutNode? _content;

    /// <summary>
    /// Uses the supplied layout content directly.
    /// </summary>
    public ContentBuilder Use(LayoutNode content)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
        return this;
    }

    /// <summary>
    /// Centers content within the available bounds.
    /// </summary>
    public ContentBuilder Center(LayoutNode content, int? width = null, int? height = null, Thickness margin = default)
    {
        _content = new CenterLayout(content, width, height, margin);
        return this;
    }

    /// <summary>
    /// Builds centered content imperatively.
    /// </summary>
    public ContentBuilder Center(Action<ContentBuilder> configure, int? width = null, int? height = null, Thickness margin = default)
    {
        _content = new CenterLayout(BuildNestedContent(configure), width, height, margin);
        return this;
    }

    /// <summary>
    /// Wraps content in a panel.
    /// </summary>
    public ContentBuilder Panel(Action<PanelBuilder> configure)
    {
        var builder = new PanelBuilder();
        configure(builder);
        _content = builder.Build();
        return this;
    }

    /// <summary>
    /// Builds a horizontal content row.
    /// </summary>
    public ContentBuilder Row(Action<StackBuilder> configure)
    {
        _content = StackBuilder.BuildRow(configure);
        return this;
    }

    /// <summary>
    /// Builds a vertical content column.
    /// </summary>
    public ContentBuilder Column(Action<StackBuilder> configure)
    {
        _content = StackBuilder.BuildColumn(configure);
        return this;
    }

    internal LayoutNode Build() =>
        _content ?? throw new InvalidOperationException("Content builder requires content to be configured.");

    private static LayoutNode BuildNestedContent(Action<ContentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new ContentBuilder();
        configure(builder);
        return builder.Build();
    }
}

/// <summary>
/// Imperative builder for row and column content.
/// </summary>
public sealed class StackBuilder
{
    private readonly RowLayout? _row;
    private readonly ColumnLayout? _column;

    private StackBuilder(RowLayout row) => _row = row;

    private StackBuilder(ColumnLayout column) => _column = column;

    /// <summary>
    /// Sets the gap between items.
    /// </summary>
    public StackBuilder Gap(int gap)
    {
        if (_row is not null)
        {
            _row.Gap = gap;
        }
        else
        {
            _column!.Gap = gap;
        }

        return this;
    }

    /// <summary>
    /// Sets inner padding for the stack.
    /// </summary>
    public StackBuilder Padding(Thickness padding)
    {
        if (_row is not null)
        {
            _row.Padding = padding;
        }
        else
        {
            _column!.Padding = padding;
        }

        return this;
    }

    /// <summary>
    /// Sets uniform inner padding for the stack.
    /// </summary>
    public StackBuilder Padding(int all) => Padding(Thickness.All(all));

    /// <summary>
    /// Adds an auto-sized item.
    /// </summary>
    public StackBuilder Auto(LayoutNode content, Thickness margin = default) =>
        Add(LayoutSlot.Auto(content, margin));

    /// <summary>
    /// Adds a fixed-size item.
    /// </summary>
    public StackBuilder Fixed(int size, LayoutNode content, Thickness margin = default) =>
        Add(LayoutSlot.Fixed(content, size, margin));

    /// <summary>
    /// Adds a fill item.
    /// </summary>
    public StackBuilder Fill(LayoutNode content, Thickness margin = default) =>
        Add(LayoutSlot.Fill(content, margin));

    /// <summary>
    /// Adds a weighted item.
    /// </summary>
    public StackBuilder Weighted(int weight, LayoutNode content, Thickness margin = default) =>
        Add(LayoutSlot.Weighted(content, weight, margin));

    /// <summary>
    /// Adds an auto-sized nested item built imperatively.
    /// </summary>
    public StackBuilder Auto(Action<ContentBuilder> configure, Thickness margin = default) =>
        Auto(BuildContent(configure), margin);

    /// <summary>
    /// Adds a fixed-size nested item built imperatively.
    /// </summary>
    public StackBuilder Fixed(int size, Action<ContentBuilder> configure, Thickness margin = default) =>
        Fixed(size, BuildContent(configure), margin);

    /// <summary>
    /// Adds a fill nested item built imperatively.
    /// </summary>
    public StackBuilder Fill(Action<ContentBuilder> configure, Thickness margin = default) =>
        Fill(BuildContent(configure), margin);

    /// <summary>
    /// Adds a weighted nested item built imperatively.
    /// </summary>
    public StackBuilder Weighted(int weight, Action<ContentBuilder> configure, Thickness margin = default) =>
        Weighted(weight, BuildContent(configure), margin);

    internal static RowLayout BuildRow(Action<StackBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var layout = new RowLayout();
        configure(new StackBuilder(layout));
        return layout;
    }

    internal static ColumnLayout BuildColumn(Action<StackBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var layout = new ColumnLayout();
        configure(new StackBuilder(layout));
        return layout;
    }

    private StackBuilder Add(LayoutSlot slot)
    {
        if (_row is not null)
        {
            _row.Items.Add(slot);
        }
        else
        {
            _column!.Items.Add(slot);
        }

        return this;
    }

    private static LayoutNode BuildContent(Action<ContentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new ContentBuilder();
        configure(builder);
        return builder.Build();
    }
}

/// <summary>
/// Imperative builder for framed panel content.
/// </summary>
public sealed class PanelBuilder
{
    private LayoutNode? _content;
    private string? _title;
    private BorderStyle _border;
    private Thickness _padding;
    private Thickness _margin;

    /// <summary>
    /// Sets the panel title.
    /// </summary>
    public PanelBuilder Title(string? title)
    {
        _title = title;
        return this;
    }

    /// <summary>
    /// Sets the panel border style.
    /// </summary>
    public PanelBuilder Border(BorderStyle style = BorderStyle.SingleLine)
    {
        _border = style;
        return this;
    }

    /// <summary>
    /// Sets the panel padding.
    /// </summary>
    public PanelBuilder Padding(Thickness padding)
    {
        _padding = padding;
        return this;
    }

    /// <summary>
    /// Sets uniform panel padding.
    /// </summary>
    public PanelBuilder Padding(int all) => Padding(Thickness.All(all));

    /// <summary>
    /// Sets the outer panel margin.
    /// </summary>
    public PanelBuilder Margin(Thickness margin)
    {
        _margin = margin;
        return this;
    }

    /// <summary>
    /// Sets the panel content directly.
    /// </summary>
    public PanelBuilder Content(LayoutNode content)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
        return this;
    }

    /// <summary>
    /// Builds the panel content imperatively.
    /// </summary>
    public PanelBuilder Content(Action<ContentBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var builder = new ContentBuilder();
        configure(builder);
        _content = builder.Build();
        return this;
    }

    internal PanelLayout Build() =>
        new(_content ?? throw new InvalidOperationException("Panel builder requires content to be configured."), _title, _border, _padding, _margin);
}
