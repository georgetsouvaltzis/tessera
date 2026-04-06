using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a generic, explicitly configured data-entry form.
/// </summary>
/// <typeparam name="TModel">Bound model type.</typeparam>
public sealed partial class DataForm<TModel> : Control
    where TModel : class
{
    private readonly List<DataFormField<TModel>> _fields = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;
    private string _editBuffer = string.Empty;
    private bool _isDirty;
    private bool _isEditing;
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
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets focused title style.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets unfocused border style.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets focused border style.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets field label style.</summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets field value style.</summary>
    public TesseraStyle ValueStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets placeholder style for empty values.</summary>
    public TesseraStyle PlaceholderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected-row style.</summary>
    public TesseraStyle SelectedFieldStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected-row style while focused.</summary>
    public TesseraStyle FocusedSelectedFieldStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets hovered-row style.</summary>
    public TesseraStyle HoveredFieldStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets read-only row style.</summary>
    public TesseraStyle ReadOnlyFieldStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets disabled style.</summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets error style for failed commit feedback.</summary>
    public TesseraStyle ErrorStyle { get; set; } = TesseraStyle.Empty;

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

    /// <summary>Gets whether the selected field is in explicit value-edit mode.</summary>
    public bool IsEditing => _isEditing;

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
        _isEditing = false;
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
        _isEditing = false;
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
            _isEditing = false;
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, _fields.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _fields.Count - 1);
            _isEditing = false;
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
        _isEditing = false;
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
    /// Enters explicit value-edit mode for the selected field.
    /// </summary>
    /// <returns><see langword="true"/> when edit mode was entered.</returns>
    public bool BeginEdit() => BeginEditCore();

    /// <summary>
    /// Commits the current edit buffer into the model.
    /// </summary>
    /// <returns><see langword="true"/> when commit was handled.</returns>
    public bool Commit() => CommitCurrentField(out _);

    /// <summary>
    /// Cancels the current edit session and restores the selected field value.
    /// </summary>
    /// <returns><see langword="true"/> when edit mode was canceled.</returns>
    public bool CancelEdit() => CancelCurrentEdit();
}
