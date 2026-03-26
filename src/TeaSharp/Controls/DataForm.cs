using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a generic, explicitly configured data-entry form.
/// </summary>
/// <typeparam name="TModel">Bound model type.</typeparam>
public sealed class DataForm<TModel> : Control
    where TModel : class
{
    private readonly List<DataFormField<TModel>> _fields = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;
    private string _editBuffer = string.Empty;
    private bool _isDirty;
    private string _lastCommitError = string.Empty;

    /// <summary>
    /// Occurs when selected field changes.
    /// </summary>
    public event EventHandler<DataFormSelectionChangedEventArgs<TModel>>? SelectionChanged;

    /// <summary>
    /// Occurs when current field commit is attempted.
    /// </summary>
    public event EventHandler<DataFormFieldCommittedEventArgs<TModel>>? FieldCommitted;

    /// <summary>Gets or sets title text.</summary>
    public string Title { get; set; } = "Data Form";
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets text shown when no fields are registered.</summary>
    public string EmptyText { get; set; } = "(no fields)";
    /// <summary>Gets or sets text shown when model is not set.</summary>
    public string NoModelText { get; set; } = "(no model)";
    /// <summary>Gets or sets selected-row marker text.</summary>
    public string SelectedMarker { get; set; } = ">";
    /// <summary>Gets or sets non-selected-row marker text.</summary>
    public string UnselectedMarker { get; set; } = " ";
    /// <summary>Gets or sets text between label and value columns.</summary>
    public string FieldSeparatorText { get; set; } = ": ";
    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Gets or sets max rendered label width.</summary>
    public int MaxLabelWidth { get; set; } = 20;

    /// <summary>Gets or sets unfocused title style.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets focused title style.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets unfocused border style.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets focused border style.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets field label style.</summary>
    public TeaStyle LabelStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets field value style.</summary>
    public TeaStyle ValueStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets placeholder style for empty values.</summary>
    public TeaStyle PlaceholderStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets selected-row style.</summary>
    public TeaStyle SelectedFieldStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets selected-row style while focused.</summary>
    public TeaStyle FocusedSelectedFieldStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets hovered-row style.</summary>
    public TeaStyle HoveredFieldStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets read-only row style.</summary>
    public TeaStyle ReadOnlyFieldStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets disabled style.</summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets error style for failed commit feedback.</summary>
    public TeaStyle ErrorStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets current bound model, when any.</summary>
    public TModel? Model { get; private set; }
    /// <summary>Gets registered field definitions.</summary>
    public IReadOnlyList<DataFormField<TModel>> Fields => _fields;
    /// <summary>Gets selected field index, or <c>-1</c> when no fields exist.</summary>
    public int SelectedIndex => _selectedIndex;
    /// <summary>Gets selected field definition, if any.</summary>
    public DataFormField<TModel>? SelectedField => _selectedIndex >= 0 && _selectedIndex < _fields.Count ? _fields[_selectedIndex] : null;
    /// <summary>Gets current edit buffer for selected field.</summary>
    public string EditBuffer => _editBuffer;
    /// <summary>Gets last commit error text.</summary>
    public string LastCommitError => _lastCommitError;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Sets bound model instance.
    /// </summary>
    /// <param name="model">Model instance to edit.</param>
    public void SetModel(TModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        LoadBufferFromSelected();
    }

    /// <summary>
    /// Clears bound model reference.
    /// </summary>
    public void ClearModel()
    {
        Model = null;
        _editBuffer = string.Empty;
        _isDirty = false;
        _lastCommitError = string.Empty;
    }

    /// <summary>
    /// Replaces all registered fields.
    /// </summary>
    /// <param name="fields">Field definitions.</param>
    public void SetFields(IEnumerable<DataFormField<TModel>> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        var previousIndex = _selectedIndex;
        var previousField = SelectedField;
        _fields.Clear();
        foreach (var field in fields)
        {
            if (field is not null)
            {
                _fields.Add(field);
            }
        }

        if (_fields.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
            _editBuffer = string.Empty;
            _isDirty = false;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _fields.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _fields.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
            LoadBufferFromSelected();
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousField);
    }

    /// <summary>
    /// Registers one field definition.
    /// </summary>
    /// <param name="field">Field definition.</param>
    public void RegisterField(DataFormField<TModel> field)
    {
        ArgumentNullException.ThrowIfNull(field);
        var previousIndex = _selectedIndex;
        var previousField = SelectedField;
        _fields.Add(field);
        if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
            LoadBufferFromSelected();
        }

        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChangedIfNeeded(previousIndex, previousField);
    }

    /// <summary>
    /// Registers one field using explicit getter/setter delegates.
    /// </summary>
    public DataFormField<TModel> RegisterField(
        string key,
        string label,
        Func<TModel, string> readValue,
        Action<TModel, string>? writeValue = null,
        string? placeholder = null,
        bool isReadOnly = false,
        Func<string, string?>? validator = null)
    {
        var field = new DataFormField<TModel>(key, label, readValue, writeValue, placeholder, isReadOnly, validator);
        RegisterField(field);
        return field;
    }

    /// <summary>
    /// Selects field by index.
    /// </summary>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SelectField(int index)
    {
        if (_fields.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _fields.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousField = SelectedField;
        _selectedIndex = clamped;
        _lastCommitError = string.Empty;
        EnsureSelectionVisible(_lastViewportRows);
        LoadBufferFromSelected();
        RaiseSelectionChangedIfNeeded(previousIndex, previousField);
        return true;
    }

    /// <summary>
    /// Selects the first field with a matching <see cref="DataFormField{TModel}.Key"/>.
    /// </summary>
    /// <param name="key">Stable field key to select.</param>
    /// <returns>
    /// <see langword="true"/> when a matching field exists and selection changed;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool SelectField(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        for (var index = 0; index < _fields.Count; index++)
        {
            if (string.Equals(_fields[index].Key, key, StringComparison.Ordinal))
            {
                return SelectField(index);
            }
        }

        return false;
    }

    /// <summary>Selects next field.</summary>
    public bool NextField() => SelectField(_selectedIndex + 1);

    /// <summary>Selects previous field.</summary>
    public bool PreviousField() => SelectField(_selectedIndex - 1);

    /// <summary>
    /// Commits current edit buffer into the model.
    /// </summary>
    /// <returns><see langword="true"/> when commit was handled.</returns>
    public bool Commit() => CommitCurrentField();

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        if (message is Pasted pasted && CanEditCurrentField())
        {
            _editBuffer += pasted.Content ?? string.Empty;
            _isDirty = true;
            _lastCommitError = string.Empty;
            return true;
        }

        if (message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return NavigateAndCommit(NextField);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return NavigateAndCommit(PreviousField);
        if (key.Is(Key.Home)) return NavigateAndCommit(() => SelectField(0));
        if (key.Is(Key.End)) return NavigateAndCommit(() => SelectField(_fields.Count - 1));
        if (key.Is(Key.PageDown)) return NavigateAndCommit(() => SelectField(_selectedIndex + Math.Max(1, _lastViewportRows)));
        if (key.Is(Key.PageUp)) return NavigateAndCommit(() => SelectField(_selectedIndex - Math.Max(1, _lastViewportRows)));
        if (key.Is(Key.Enter)) return CommitCurrentField();
        if (key.Is(Key.Escape)) return CancelCurrentEdit();
        if (!CanEditCurrentField()) return false;
        if (key.Is(Key.Backspace)) return RemoveFromBuffer(backspace: true);
        if (key.Is(Key.Delete)) return RemoveFromBuffer(backspace: false);

        if (key.Key == Key.Character
            && !string.IsNullOrEmpty(key.Text)
            && !key.Modifiers.HasFlag(ModifierKeys.Ctrl)
            && !key.Modifiers.HasFlag(ModifierKeys.Alt)
            && !key.Modifiers.HasFlag(ModifierKeys.Meta))
        {
            _editBuffer += key.Text;
            _isDirty = true;
            _lastCommitError = string.Empty;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var rowTop = ResolveRowsTop(content);
        var rowsHeight = Math.Max(0, content.Bottom - rowTop);
        _lastViewportRows = Math.Max(1, rowsHeight);

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHovered(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _fields.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return NavigateAndCommit(NextField) || changed;
            if (pointer.Button == PointerButton.WheelUp) return NavigateAndCommit(PreviousField) || changed;
        }

        if (!inside || _fields.Count == 0 || pointer.Y < rowTop || rowsHeight <= 0)
        {
            return changed || Handle(message);
        }

        EnsureSelectionVisible(rowsHeight);
        var hovered = _scrollOffset + (pointer.Y - rowTop);
        if (hovered < 0 || hovered >= _fields.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHovered(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHovered(hovered);
            changed |= NavigateAndCommit(() => SelectField(hovered));
        }

        return changed;
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
            Border == BorderStyle.None ? null : RenderTitleFrameText(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        var y = content.Y;
        if (ShouldRenderInlineTitle())
        {
            WriteStyledText(canvas, content.X, y, RenderTitleText(), ResolveTitleStyle(), content.Width);
            y++;
        }

        var rowsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, rowsHeight);
        if (_fields.Count == 0 || rowsHeight <= 0)
        {
            if (rowsHeight > 0)
            {
                WriteStyledText(canvas, content.X, y, EmptyText, ResolveEmptyStyle(), content.Width);
            }

            return;
        }

        EnsureSelectionVisible(rowsHeight);
        var labelWidth = ResolveLabelWidth();
        var visible = Math.Min(rowsHeight, _fields.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var index = _scrollOffset + row;
            RenderFieldRow(canvas, content.X, y + row, content.Width, labelWidth, index, _fields[index]);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var labelWidth = ResolveLabelWidth();
        var width = Math.Max(24, labelWidth + 18 + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        if (ShouldRenderInlineTitle() || !string.IsNullOrWhiteSpace(Title))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(RenderTitleText()) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2));
        }

        var height = Math.Max(1, _fields.Count) + (ShouldRenderInlineTitle() ? 1 : 0) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderFieldRow(Canvas canvas, int x, int y, int width, int labelWidth, int index, DataFormField<TModel> field)
    {
        var rowStyle = ResolveFieldStyle(index, field);
        var marker = index == _selectedIndex ? SelectedMarker : UnselectedMarker;
        var label = NormalizeSingleLine(field.Label).PadRight(labelWidth);
        var prefix = string.Concat(marker, " ", label, FieldSeparatorText);
        var prefixStyle = rowStyle.Merge(LabelStyle);
        WriteStyledText(canvas, x, y, prefix, prefixStyle, width);

        var prefixWidth = Math.Min(width, ControlTextLayout.MeasureDisplayWidth(prefix));
        if (prefixWidth >= width)
        {
            return;
        }

        var value = ResolveDisplayedValue(index, field, out var isPlaceholder);
        if (index == _selectedIndex && !string.IsNullOrWhiteSpace(_lastCommitError))
        {
            value = string.Concat(value, " ! ", _lastCommitError);
        }

        var valueStyle = rowStyle.Merge(isPlaceholder ? PlaceholderStyle : ValueStyle);
        if (index == _selectedIndex && !string.IsNullOrWhiteSpace(_lastCommitError))
        {
            valueStyle = valueStyle.Merge(ErrorStyle);
        }

        WriteStyledText(canvas, x + prefixWidth, y, value, valueStyle, width - prefixWidth);
    }

    private bool NavigateAndCommit(Func<bool> move)
    {
        var committed = CommitCurrentField();
        var moved = move();
        return committed || moved;
    }

    private bool CommitCurrentField()
    {
        if (!CanEditCurrentField() || Model is null || SelectedField is null)
        {
            return false;
        }

        var previousValue = SafeReadValue(SelectedField, Model);
        if (!_isDirty && string.Equals(previousValue, _editBuffer, StringComparison.Ordinal))
        {
            return false;
        }

        var committedValue = _editBuffer;
        var success = SelectedField.TryCommit(Model, committedValue, out var error);
        _lastCommitError = success ? string.Empty : (error ?? "Commit failed.");
        _isDirty = !success;

        if (success)
        {
            _editBuffer = SafeReadValue(SelectedField, Model);
        }

        FieldCommitted?.Invoke(
            this,
            new DataFormFieldCommittedEventArgs<TModel>(Model, _selectedIndex, SelectedField, previousValue, committedValue, success, _lastCommitError));
        return true;
    }

    private bool CancelCurrentEdit()
    {
        if (!CanEditCurrentField())
        {
            return false;
        }

        var previous = _editBuffer;
        LoadBufferFromSelected();
        return !string.Equals(previous, _editBuffer, StringComparison.Ordinal);
    }

    private bool RemoveFromBuffer(bool backspace)
    {
        if (_editBuffer.Length == 0)
        {
            return false;
        }

        if (backspace)
        {
            _editBuffer = _editBuffer[..^1];
        }
        else
        {
            _editBuffer = _editBuffer.Length == 1 ? string.Empty : _editBuffer[1..];
        }

        _isDirty = true;
        _lastCommitError = string.Empty;
        return true;
    }

    private bool CanEditCurrentField()
    {
        return !IsReadOnly && SelectedField is { CanWrite: true } && Model is not null;
    }

    private bool SetHovered(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private int ResolveRowsTop(Rect content)
    {
        return ShouldRenderInlineTitle() ? content.Y + 1 : content.Y;
    }

    private bool ShouldRenderInlineTitle() => Border == BorderStyle.None && !string.IsNullOrWhiteSpace(Title);

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_fields.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
        }
        else if (_selectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _fields.Count - viewportRows));
    }

    private int ResolveLabelWidth()
    {
        var width = 6;
        for (var index = 0; index < _fields.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(_fields[index].Label));
        }

        return Math.Clamp(width, 4, Math.Max(4, MaxLabelWidth));
    }

    private string ResolveDisplayedValue(int index, DataFormField<TModel> field, out bool isPlaceholder)
    {
        if (index == _selectedIndex)
        {
            var current = _editBuffer;
            if (string.IsNullOrEmpty(current))
            {
                var placeholder = string.IsNullOrWhiteSpace(field.Placeholder) ? NoModelText : field.Placeholder;
                isPlaceholder = true;
                return placeholder;
            }

            isPlaceholder = false;
            return current;
        }

        if (Model is null)
        {
            isPlaceholder = true;
            return NoModelText;
        }

        var value = SafeReadValue(field, Model);
        if (string.IsNullOrEmpty(value))
        {
            var placeholder = string.IsNullOrWhiteSpace(field.Placeholder) ? NoModelText : field.Placeholder;
            isPlaceholder = true;
            return placeholder;
        }

        isPlaceholder = false;
        return value;
    }

    private void LoadBufferFromSelected()
    {
        _isDirty = false;
        _lastCommitError = string.Empty;
        if (Model is null || SelectedField is null)
        {
            _editBuffer = string.Empty;
            return;
        }

        _editBuffer = SafeReadValue(SelectedField, Model);
    }

    private static string SafeReadValue(DataFormField<TModel> field, TModel model)
    {
        try
        {
            return field.ReadValue(model) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TeaStyle ResolveTitleStyle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TeaStyle ResolveFieldStyle(int index, DataFormField<TModel> field)
    {
        var style = TeaStyle.Empty;
        if (field.IsReadOnly || !field.CanWrite) style = style.Merge(ReadOnlyFieldStyle);
        if (index == _hoveredIndex) style = style.Merge(HoveredFieldStyle);
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedFieldStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedFieldStyle);
        }

        if (IsDisabled) style = style.Merge(DisabledStyle);
        return style;
    }

    private TeaStyle ResolveEmptyStyle()
    {
        return IsDisabled ? PlaceholderStyle.Merge(DisabledStyle) : PlaceholderStyle;
    }

    private string RenderTitleText()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }

        return Title;
    }

    private string RenderTitleFrameText()
    {
        var title = RenderTitleText();
        var style = ResolveTitleStyle();
        return style.IsEmpty ? title : style.Render(title);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, DataFormField<TModel>? previousField)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousField, SelectedField))
        {
            return;
        }

        SelectionChanged?.Invoke(this, new DataFormSelectionChangedEventArgs<TModel>(previousIndex, _selectedIndex, previousField, SelectedField));
    }

    private static string NormalizeSingleLine(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace('\r', ' ').Replace('\n', ' ');
    }

    private static void WriteStyledText(Canvas canvas, int x, int y, string text, TeaStyle style, int width)
    {
        if (width <= 0)
        {
            return;
        }

        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
    }
}
