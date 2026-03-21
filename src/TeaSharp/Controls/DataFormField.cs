namespace TeaSharp.Controls;

/// <summary>
/// Represents one explicitly registered field in a <see cref="DataForm{TModel}"/>.
/// </summary>
/// <typeparam name="TModel">Bound model type.</typeparam>
public sealed class DataFormField<TModel>
{
    /// <summary>
    /// Initializes a field definition.
    /// </summary>
    /// <param name="key">Stable field key.</param>
    /// <param name="label">Display label text.</param>
    /// <param name="readValue">Delegate that reads current text value from model.</param>
    /// <param name="writeValue">Delegate that writes committed text value to model.</param>
    /// <param name="placeholder">Optional placeholder text shown for empty values.</param>
    /// <param name="isReadOnly">Whether this field is read-only.</param>
    /// <param name="validator">Optional validator returning error text when invalid.</param>
    public DataFormField(
        string key,
        string label,
        Func<TModel, string> readValue,
        Action<TModel, string>? writeValue = null,
        string? placeholder = null,
        bool isReadOnly = false,
        Func<string, string?>? validator = null)
    {
        ArgumentNullException.ThrowIfNull(readValue);
        Key = key ?? string.Empty;
        Label = label ?? string.Empty;
        ReadValue = readValue;
        WriteValue = writeValue;
        Placeholder = placeholder ?? string.Empty;
        IsReadOnly = isReadOnly;
        Validator = validator;
    }

    /// <summary>Gets stable field key.</summary>
    public string Key { get; }

    /// <summary>Gets display label text.</summary>
    public string Label { get; }

    /// <summary>Gets placeholder text used for empty values.</summary>
    public string Placeholder { get; }

    /// <summary>Gets whether this field is read-only.</summary>
    public bool IsReadOnly { get; }

    /// <summary>Gets delegate that reads current text value from model.</summary>
    public Func<TModel, string> ReadValue { get; }

    /// <summary>Gets delegate that writes committed text value to model.</summary>
    public Action<TModel, string>? WriteValue { get; }

    /// <summary>Gets optional validator for committed text values.</summary>
    public Func<string, string?>? Validator { get; }

    /// <summary>Gets whether this field can commit edits.</summary>
    public bool CanWrite => !IsReadOnly && WriteValue is not null;

    internal bool TryCommit(TModel model, string value, out string? error)
    {
        if (!CanWrite)
        {
            error = "Field is read-only.";
            return false;
        }

        error = Validator?.Invoke(value);
        if (!string.IsNullOrEmpty(error))
        {
            return false;
        }

        try
        {
            WriteValue!(model, value);
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }
}

/// <summary>
/// Provides previous/current values when <see cref="DataForm{TModel}.SelectionChanged"/> fires.
/// </summary>
/// <typeparam name="TModel">Bound model type.</typeparam>
public sealed class DataFormSelectionChangedEventArgs<TModel> : EventArgs
{
    /// <summary>
    /// Initializes selection-change payload.
    /// </summary>
    /// <param name="previousIndex">Previously selected field index.</param>
    /// <param name="selectedIndex">Current selected field index.</param>
    /// <param name="previousField">Previously selected field.</param>
    /// <param name="selectedField">Current selected field.</param>
    public DataFormSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        DataFormField<TModel>? previousField,
        DataFormField<TModel>? selectedField)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousField = previousField;
        SelectedField = selectedField;
    }

    /// <summary>Gets previously selected field index.</summary>
    public int PreviousIndex { get; }

    /// <summary>Gets current selected field index.</summary>
    public int SelectedIndex { get; }

    /// <summary>Gets previously selected field.</summary>
    public DataFormField<TModel>? PreviousField { get; }

    /// <summary>Gets current selected field.</summary>
    public DataFormField<TModel>? SelectedField { get; }
}

/// <summary>
/// Provides commit results when <see cref="DataForm{TModel}.FieldCommitted"/> fires.
/// </summary>
/// <typeparam name="TModel">Bound model type.</typeparam>
public sealed class DataFormFieldCommittedEventArgs<TModel> : EventArgs
{
    /// <summary>
    /// Initializes commit payload.
    /// </summary>
    /// <param name="model">Bound model instance.</param>
    /// <param name="fieldIndex">Committed field index.</param>
    /// <param name="field">Committed field definition.</param>
    /// <param name="previousValue">Value before commit.</param>
    /// <param name="committedValue">Value requested for commit.</param>
    /// <param name="success">Whether commit succeeded.</param>
    /// <param name="error">Optional error text when commit failed.</param>
    public DataFormFieldCommittedEventArgs(
        TModel model,
        int fieldIndex,
        DataFormField<TModel> field,
        string previousValue,
        string committedValue,
        bool success,
        string? error = null)
    {
        Model = model;
        FieldIndex = fieldIndex;
        Field = field;
        PreviousValue = previousValue;
        CommittedValue = committedValue;
        Success = success;
        Error = error ?? string.Empty;
    }

    /// <summary>Gets bound model instance.</summary>
    public TModel Model { get; }

    /// <summary>Gets committed field index.</summary>
    public int FieldIndex { get; }

    /// <summary>Gets committed field definition.</summary>
    public DataFormField<TModel> Field { get; }

    /// <summary>Gets field value before commit.</summary>
    public string PreviousValue { get; }

    /// <summary>Gets value requested for commit.</summary>
    public string CommittedValue { get; }

    /// <summary>Gets whether commit succeeded.</summary>
    public bool Success { get; }

    /// <summary>Gets commit error text when <see cref="Success"/> is <see langword="false"/>.</summary>
    public string Error { get; }
}
