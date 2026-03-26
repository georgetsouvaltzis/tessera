# ConsumerWorkflowLab Friction Notes

Scope: consumer-only usage of `TeaSharp` + `TeaSharp.Controls` + `TeaSharp.Layout` + `TeaSharp.Styles`.

## 1) `Choice` has no direct programmatic selection API

Observed friction:
- No `SetSelectedIndex`/`SetSelectedItem` on `Choice`.
- Consumer code must synthesize key events and focus state to drive selection.

Exact workaround used in app:

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

    var previousFocus = _environmentChoice.IsFocused;
    _environmentChoice.IsFocused = true;

    _environmentChoice.Handle(new KeyPressed(Key.Enter));
    var currentIndex = Array.IndexOf(EnvironmentOptions, _environmentChoice.SelectedItem);
    if (currentIndex < 0)
    {
        currentIndex = 0;
    }

    var targetIndex = Array.IndexOf(EnvironmentOptions, target);
    var forwardSteps = (targetIndex - currentIndex + EnvironmentOptions.Length) % EnvironmentOptions.Length;
    for (var step = 0; step < forwardSteps; step++)
    {
        _environmentChoice.Handle(new KeyPressed(Key.Down));
    }

    _environmentChoice.Handle(new KeyPressed(Key.Enter));
    _environmentChoice.IsFocused = previousFocus;

    if (string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal))
    {
        return true;
    }

    _environmentChoice.SetItems(Array.Empty<string>());
    _environmentChoice.SetItems(BuildPreferredFirstOrder(target, EnvironmentOptions));
    return string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal);
}
```

Additive API candidate:
- `bool Choice.SetSelectedIndex(int index)`
- `bool Choice.TrySetSelectedItem(string item)`

Type: additive (no deeper redesign required).

## 2) `ComboBox` has no direct programmatic selection API

Observed friction:
- No `SetSelectedIndex`/`SetSelectedItem` on `ComboBox`.
- Consumer must combine `SetFilterText`, synthetic key events, and focus toggles.

Exact workaround used in app:

```csharp
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

    var previousFocus = _templateCombo.IsFocused;
    _templateCombo.IsFocused = true;
    _templateCombo.SetFilterText(target);
    _templateCombo.Handle(new KeyPressed(Key.Down));
    _templateCombo.Handle(new KeyPressed(Key.Enter));
    _templateCombo.SetFilterText(string.Empty);
    _templateCombo.IsFocused = previousFocus;

    if (string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal))
    {
        return true;
    }

    _templateCombo.SetItems(Array.Empty<string>());
    _templateCombo.SetItems(BuildPreferredFirstOrder(target, TemplateOptions));
    _templateCombo.IsFocused = true;
    _templateCombo.Handle(new KeyPressed(Key.Down));
    _templateCombo.Handle(new KeyPressed(Key.Enter));
    _templateCombo.IsFocused = previousFocus;
    return string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal);
}
```

Additive API candidate:
- `bool ComboBox.SetSelectedIndex(int index)`
- `bool ComboBox.TrySetSelectedItem(string item)`
- optional: `void ComboBox.Open()` for explicit consumer control

Type: additive.

## 3) `DataForm<T>` keyed selection requires local index map

Observed friction:
- `DataForm<T>` exposes `SelectField(int index)` only.
- Validation issue routing and shortcut targeting by field key needs custom `Dictionary<string,int>` bookkeeping.

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

Type: additive.

## 4) Theme override repetition across workflow/form controls

Observed friction:
- Applying consistent title/border/focus/selected styles across many controls requires repetitive per-control code.
- Existing `ThemeScope.Apply` helps base theme fan-out but not common override bundles for forms/workflow surfaces.

Potential additive API candidate:
- `TeaThemeOverrideBundle` extension set for forms/workflow controls (`DataForm`, `ValidationSummary`, `Wizard`, `Stepper`, `FieldSet`, `Form`).

Type: additive.
