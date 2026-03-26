# ConsumerWorkflowLab Friction Notes

Scope: consumer-only usage of `TeaSharp` + `TeaSharp.Controls` + `TeaSharp.Layout` + `TeaSharp.Styles`.

## Closed in this lane: Choice/ComboBox programmatic selection

New additive APIs now remove synthetic key-event workarounds:
- `bool Choice.SetSelectedIndex(int index)`
- `bool Choice.TrySetSelectedItem(string item)`
- `bool ComboBox.SetSelectedIndex(int index)`
- `bool ComboBox.TrySetSelectedItem(string item)`

Exact selection code now used by the app:

```csharp
private bool ForceChoiceSelection(string target)
{
    if (!EnvironmentOptions.Contains(target, StringComparer.Ordinal))
    {
        return false;
    }

    if (string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal))
    {
        return true;
    }

    return _environmentChoice.TrySetSelectedItem(target)
        || string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal);
}

private bool ForceComboSelection(string target)
{
    if (!TemplateOptions.Contains(target, StringComparer.Ordinal))
    {
        return false;
    }

    if (string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal))
    {
        return true;
    }

    return _templateCombo.TrySetSelectedItem(target)
        || string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal);
}
```

Remaining friction:
- none for Choice/ComboBox selection ergonomics on this scenario.

## Remaining: `DataForm<T>` keyed selection still requires a local key-map

Observed friction:
- `DataForm<T>` exposes `SelectField(int index)` only.
- Validation issue routing and shortcut targeting by field key still needs custom `Dictionary<string,int>` bookkeeping.

Exact workaround used in app:

```csharp
private readonly Dictionary<string, int> _fieldIndexByKey = new(StringComparer.Ordinal);

private void RegisterDataField(
    string key,
    string label,
    Func<WorkflowDraft, string> readValue,
    Action<WorkflowDraft, string> writeValue,
    string placeholder,
    Func<string, string?> validator)
{
    _fieldIndexByKey[key] = _fieldIndexByKey.Count;
    _dataForm.RegisterField(key, label, readValue, writeValue, placeholder: placeholder, validator: validator);
}

private bool SelectDataFormFieldByKey(string key)
{
    if (!_fieldIndexByKey.TryGetValue(key, out var index))
    {
        return false;
    }

    _dataForm.RequestFocus();
    var changed = _dataForm.SelectField(index);
    return changed || string.Equals(_dataForm.SelectedField?.Key, key, StringComparison.Ordinal);
}
```

Additive API candidate:
- `bool DataForm<T>.SelectField(string key)`
- optional: `bool DataForm<T>.TryGetFieldIndex(string key, out int index)`
