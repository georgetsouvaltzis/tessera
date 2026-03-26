# ConsumerWorkflowLab Friction Notes

Scope: consumer-only usage of `TeaSharp` + `TeaSharp.Controls` + `TeaSharp.Layout` + `TeaSharp.Styles`.

## Resolved in current HEAD

### Choice/ComboBox programmatic selection

Additive APIs now used by app code:
- `bool Choice.SetSelectedIndex(int index)`
- `bool Choice.TrySetSelectedItem(string item)`
- `bool ComboBox.SetSelectedIndex(int index)`
- `bool ComboBox.TrySetSelectedItem(string item)`

### DataForm keyed selection

`DataForm<T>.SelectField(string key)` removes local key-to-index bookkeeping.

Current app code:

```csharp
private bool SelectDataFormFieldByKey(string key)
{
    if (string.IsNullOrWhiteSpace(key))
    {
        return false;
    }

    _dataForm.RequestFocus();
    var changed = _dataForm.SelectField(key);
    return changed || string.Equals(_dataForm.SelectedField?.Key, key, StringComparison.Ordinal);
}
```

## Still-open friction (this app)

- None observed in this selection-ergonomics scenario after adopting the new APIs.
