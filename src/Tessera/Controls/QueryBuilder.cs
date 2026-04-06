using System.Text;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Interactive query composition control for rule-based filtering.
/// </summary>
public sealed class QueryBuilder : Control
{
    private readonly QueryGroup _group = new();
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private string _queryCache = string.Empty;
    private bool _queryDirty = true;

    /// <summary>
    /// Occurs when query composition changes.
    /// </summary>
    public event EventHandler<QueryChangedEventArgs>? QueryChanged;

    /// <summary>
    /// Gets or sets title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Query Builder";

    /// <summary>
    /// Gets or sets marker appended to <see cref="Title"/> while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether <see cref="FocusMarker"/> is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets marker for selected rule rows.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "▸";

    /// <summary>
    /// Gets or sets marker for non-selected rule rows.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets text rendered when no rules exist.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no rules)";

    /// <summary>
    /// Gets or sets whether preview row should be shown.
    /// </summary>
    public bool ShowQueryPreview { get; set; } = true;

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets title style while unfocused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets rule base style.
    /// </summary>
    public TesseraStyle RuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rules.
    /// </summary>
    public TesseraStyle SelectedRuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rules while focused.
    /// </summary>
    public TesseraStyle FocusedRuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rules.
    /// </summary>
    public TesseraStyle HoveredRuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when disabled.
    /// </summary>
    public TesseraStyle DisabledRuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets style merged when error state is active.
    /// </summary>
    public TesseraStyle ErrorRuleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets preview-row style.
    /// </summary>
    public TesseraStyle PreviewStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style while unfocused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets border style while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets whether control error state is active.
    /// </summary>
    public bool HasError { get; set; }

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets current query rules.
    /// </summary>
    public IReadOnlyList<QueryRule> Rules => _group.Rules;

    /// <summary>
    /// Gets current selected rule index.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets current selected rule.
    /// </summary>
    public QueryRule? SelectedRule => _selectedIndex >= 0 && _selectedIndex < _group.Count ? _group.Rules[_selectedIndex] : null;

    /// <summary>
    /// Gets or sets whether group combinator is OR.
    /// </summary>
    public bool UseOr
    {
        get => _group.UseOr;
        set => SetCombinator(value);
    }

    /// <summary>
    /// Gets current query text.
    /// </summary>
    public string QueryText => EnsureQueryText();

    /// <summary>
    /// Replaces rules.
    /// </summary>
    /// <param name="rules">Rules to set.</param>
    public void SetRules(IEnumerable<QueryRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        var previous = EnsureQueryText();
        _group.SetRules(rules);
        NormalizeSelection();
        OnRulesChanged(previous);
    }

    /// <summary>
    /// Adds one rule.
    /// </summary>
    /// <param name="field">Rule field.</param>
    /// <param name="operator">Rule operator.</param>
    /// <param name="value">Optional rule value.</param>
    /// <returns>Added rule index.</returns>
    public int AddRule(string field, QueryOperator @operator, string? value = null)
    {
        return AddRule(new QueryRule(field, @operator, value));
    }

    /// <summary>
    /// Adds one rule.
    /// </summary>
    /// <param name="rule">Rule to add.</param>
    /// <returns>Added rule index.</returns>
    public int AddRule(QueryRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        var previous = EnsureQueryText();
        _group.AddRule(rule);
        if (_selectedIndex < 0)
        {
            _selectedIndex = _group.Count - 1;
        }

        OnRulesChanged(previous);
        return _group.Count - 1;
    }

    /// <summary>
    /// Removes one rule by index.
    /// </summary>
    /// <param name="index">Rule index.</param>
    /// <returns><see langword="true"/> when removed.</returns>
    public bool RemoveRuleAt(int index)
    {
        if ((uint)index >= (uint)_group.Count)
        {
            return false;
        }

        var previous = EnsureQueryText();
        if (!_group.RemoveRuleAt(index))
        {
            return false;
        }

        NormalizeSelection();
        OnRulesChanged(previous);
        return true;
    }

    /// <summary>
    /// Clears all rules.
    /// </summary>
    public void ClearRules()
    {
        if (_group.Count == 0)
        {
            return;
        }

        var previous = EnsureQueryText();
        _group.ClearRules();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        OnRulesChanged(previous);
    }

    /// <summary>
    /// Updates one rule by index.
    /// </summary>
    /// <param name="index">Rule index.</param>
    /// <param name="field">Optional replacement field.</param>
    /// <param name="operator">Optional replacement operator.</param>
    /// <param name="value">Optional replacement value.</param>
    /// <param name="isDisabled">Optional replacement disabled state.</param>
    /// <param name="hasError">Optional replacement error state.</param>
    /// <returns><see langword="true"/> when any value changed.</returns>
    public bool UpdateRule(
        int index,
        string? field = null,
        QueryOperator? @operator = null,
        string? value = null,
        bool? isDisabled = null,
        bool? hasError = null)
    {
        if ((uint)index >= (uint)_group.Count)
        {
            return false;
        }

        var rule = _group.Rules[index];
        var changed = false;
        var previous = EnsureQueryText();
        if (field is not null && !string.Equals(field, rule.Field, StringComparison.Ordinal))
        {
            rule.Field = field;
            changed = true;
        }

        if (@operator.HasValue && @operator.Value != rule.Operator)
        {
            rule.Operator = @operator.Value;
            changed = true;
        }

        if (value is not null && !string.Equals(value, rule.Value, StringComparison.Ordinal))
        {
            rule.Value = value;
            changed = true;
        }

        if (isDisabled.HasValue && isDisabled.Value != rule.IsDisabled)
        {
            rule.IsDisabled = isDisabled.Value;
            changed = true;
        }

        if (hasError.HasValue && hasError.Value != rule.HasError)
        {
            rule.HasError = hasError.Value;
            changed = true;
        }

        if (changed)
        {
            OnRulesChanged(previous);
        }

        return changed;
    }

    /// <summary>
    /// Sets selected rule index.
    /// </summary>
    /// <param name="index">Index to select.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        var normalized = _group.Count == 0 ? -1 : Math.Clamp(index, 0, _group.Count - 1);
        if (normalized == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = normalized;
        return true;
    }

    /// <summary>
    /// Toggles current combinator.
    /// </summary>
    public void ToggleCombinator()
    {
        SetCombinator(!_group.UseOr);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.IsCharacter('t'))
        {
            ToggleCombinator();
            return true;
        }

        if (_group.Count == 0)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_group.Count - 1);
        }

        if (key.Is(Key.Backspace) || key.Is(Key.Delete))
        {
            return RemoveRuleAt(_selectedIndex);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _group.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            return pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
                ? SetHoveredIndex(-1)
                : false;
        }

        var rulesTop = ShowQueryPreview && content.Height > 1 ? content.Y + 1 : content.Y;
        var row = pointer.Y - rulesTop;
        var hit = row >= 0 && row < _group.Count ? row : -1;
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hit);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            var changed = SetHoveredIndex(hit);
            if (hit >= 0)
            {
                changed |= SetSelectedIndex(hit);
            }

            return changed || true;
        }

        return false;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        var rulesTop = content.Y;
        var rulesHeight = content.Height;
        if (ShowQueryPreview && content.Height > 1)
        {
            var combinator = _group.UseOr ? "OR" : "AND";
            var query = EnsureQueryText();
            var preview = query.Length == 0 ? $"{combinator} (empty query)" : $"{combinator} {query}";
            canvas.WriteText(content.X, content.Y, ApplyStyle(preview, ResolvePreviewStyle()), content.Width);
            rulesTop++;
            rulesHeight--;
        }

        if (rulesHeight <= 0)
        {
            return;
        }

        if (_group.Count == 0)
        {
            canvas.WriteText(content.X, rulesTop, ApplyStyle(EmptyText, ResolveRuleStyle(-1, -1, null)), content.Width);
            return;
        }

        var visibleRules = Math.Min(_group.Count, rulesHeight);
        for (var index = 0; index < visibleRules; index++)
        {
            var rule = _group.Rules[index];
            var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
            var line = BuildDisplayLine(marker, rule);
            canvas.WriteText(
                content.X,
                rulesTop + index,
                ApplyStyle(line, ResolveRuleStyle(index, _hoveredIndex, rule)),
                content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(26, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        for (var index = 0; index < _group.Count; index++)
        {
            var markerWidth = Math.Max(ControlTextLayout.MeasureDisplayWidth(SelectedMarker), ControlTextLayout.MeasureDisplayWidth(UnselectedMarker));
            var rule = _group.Rules[index];
            var rowWidth = markerWidth + 1 + ControlTextLayout.MeasureDisplayWidth(rule.Field) + 1 + ControlTextLayout.MeasureDisplayWidth(OperatorText(rule.Operator));
            if (rule.RequiresValue)
            {
                rowWidth += 1 + ControlTextLayout.MeasureDisplayWidth(rule.Value);
            }

            width = Math.Max(width, rowWidth + Padding.Horizontal + (Border != BorderStyle.None ? 2 : 0));
        }

        var height = Math.Max(4, _group.Count + Padding.Vertical + (ShowQueryPreview ? 2 : 1));
        if (Border != BorderStyle.None)
        {
            height += 2;
        }

        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void SetCombinator(bool useOr)
    {
        if (_group.UseOr == useOr)
        {
            return;
        }

        var previous = EnsureQueryText();
        _group.UseOr = useOr;
        OnRulesChanged(previous);
    }

    private void NormalizeSelection()
    {
        if (_group.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = _selectedIndex < 0 ? 0 : Math.Clamp(_selectedIndex, 0, _group.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _group.Count - 1);
    }

    private void OnRulesChanged(string previousQuery)
    {
        _queryDirty = true;
        var current = EnsureQueryText();
        if (string.Equals(previousQuery, current, StringComparison.Ordinal))
        {
            return;
        }

        QueryChanged?.Invoke(this, new QueryChangedEventArgs(previousQuery, current, _group.Count, _group.UseOr));
    }

    private string EnsureQueryText()
    {
        if (!_queryDirty)
        {
            return _queryCache;
        }

        var text = BuildQueryText();
        _queryCache = text;
        _queryDirty = false;
        return text;
    }

    private string BuildQueryText()
    {
        if (_group.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(_group.Count * 24);
        var glue = _group.UseOr ? " OR " : " AND ";
        for (var index = 0; index < _group.Count; index++)
        {
            var rule = _group.Rules[index];
            if (index > 0)
            {
                builder.Append(glue);
            }

            AppendRuleExpression(builder, rule);
        }

        return builder.ToString();
    }

    private static void AppendRuleExpression(StringBuilder builder, QueryRule rule)
    {
        builder.Append(rule.Field);
        builder.Append(' ');
        builder.Append(OperatorText(rule.Operator));
        if (!rule.RequiresValue)
        {
            return;
        }

        builder.Append(' ');
        AppendValue(builder, rule.Value);
    }

    private static void AppendValue(StringBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            builder.Append("\"\"");
            return;
        }

        var needsQuotes = false;
        for (var index = 0; index < value.Length; index++)
        {
            var ch = value[index];
            if (char.IsWhiteSpace(ch) || ch is '"' or '\'' or ',' or '(' or ')')
            {
                needsQuotes = true;
                break;
            }
        }

        if (!needsQuotes)
        {
            builder.Append(value);
            return;
        }

        builder.Append('"');
        for (var index = 0; index < value.Length; index++)
        {
            var ch = value[index];
            if (ch == '"')
            {
                builder.Append('\\');
            }

            builder.Append(ch);
        }

        builder.Append('"');
    }

    private static string BuildDisplayLine(string marker, QueryRule rule)
    {
        if (!rule.RequiresValue)
        {
            return $"{marker} {rule.Field} {OperatorText(rule.Operator)}";
        }

        return $"{marker} {rule.Field} {OperatorText(rule.Operator)} {rule.Value}";
    }

    private TesseraStyle ResolveRuleStyle(int index, int hoveredIndex, QueryRule? rule)
    {
        var style = RuleStyle;
        if (index >= 0 && index == _selectedIndex)
        {
            style = style.Merge(SelectedRuleStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedRuleStyle);
            }
        }

        if (index >= 0 && index == hoveredIndex)
        {
            style = style.Merge(HoveredRuleStyle);
        }

        if (IsDisabled || rule?.IsDisabled is true)
        {
            style = style.Merge(DisabledRuleStyle);
        }

        if (HasError || rule?.HasError is true)
        {
            style = style.Merge(ErrorRuleStyle);
        }

        return style;
    }

    private TesseraStyle ResolvePreviewStyle()
    {
        var style = PreviewStyle;
        if (HasError)
        {
            style = style.Merge(ErrorRuleStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledRuleStyle);
        }

        return style;
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledRuleStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorRuleStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = FormatTitleForMeasure();
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleForMeasure()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private bool SetHoveredIndex(int index)
    {
        var normalized = _group.Count == 0 ? -1 : Math.Clamp(index, -1, _group.Count - 1);
        if (normalized == _hoveredIndex)
        {
            return false;
        }

        _hoveredIndex = normalized;
        return true;
    }

    private static string OperatorText(QueryOperator @operator)
    {
        return @operator switch
        {
            QueryOperator.Equals => "=",
            QueryOperator.NotEquals => "!=",
            QueryOperator.Contains => "~",
            QueryOperator.StartsWith => "^=",
            QueryOperator.EndsWith => "$=",
            QueryOperator.GreaterThan => ">",
            QueryOperator.GreaterThanOrEqual => ">=",
            QueryOperator.LessThan => "<",
            QueryOperator.LessThanOrEqual => "<=",
            QueryOperator.In => "IN",
            QueryOperator.NotIn => "NOT IN",
            QueryOperator.IsEmpty => "IS EMPTY",
            QueryOperator.IsNotEmpty => "IS NOT EMPTY",
            _ => "=",
        };
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }
}
