using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;

namespace Tessera.Controls;

public sealed partial class DataForm<TModel>
{
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        if (message is Pasted pasted)
        {
            return HandlePaste(pasted);
        }

        if (message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return NavigateFromCurrentSelection(NextField);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return NavigateFromCurrentSelection(PreviousField);
        if (key.Is(Key.Home)) return NavigateFromCurrentSelection(() => SelectField(0));
        if (key.Is(Key.End)) return NavigateFromCurrentSelection(() => SelectField(_fields.Count - 1));
        if (key.Is(Key.PageDown)) return NavigateFromCurrentSelection(() => SelectField(_selectedIndex + Math.Max(1, _lastViewportRows)));
        if (key.Is(Key.PageUp)) return NavigateFromCurrentSelection(() => SelectField(_selectedIndex - Math.Max(1, _lastViewportRows)));
        if (key.Is(Key.Enter)) return _isEditing ? CommitCurrentField(out _) : BeginEditCore();
        if (key.Is(Key.Escape)) return CancelCurrentEdit();
        if (!_isEditing || !CanEditCurrentField()) return false;
        if (key.Is(Key.Backspace)) return RemoveFromBuffer();
        if (key.Is(Key.Delete)) return RemoveFromBuffer();

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
        var rowsHeight = ResolveRenderableRowsHeight(content, rowTop);
        _lastViewportRows = Math.Max(1, rowsHeight);

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHovered(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _fields.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return NavigateFromCurrentSelection(NextField) || changed;
            if (pointer.Button == PointerButton.WheelUp) return NavigateFromCurrentSelection(PreviousField) || changed;
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
            changed |= NavigateFromCurrentSelection(() => SelectField(hovered));
        }

        return changed;
    }

    private bool HandlePaste(Pasted pasted)
    {
        if (!_isEditing || !CanEditCurrentField())
        {
            return false;
        }

        _editBuffer += pasted.Content ?? string.Empty;
        _isDirty = true;
        _lastCommitError = string.Empty;
        return true;
    }

    private bool BeginEditCore()
    {
        if (_isEditing || !CanEditCurrentField())
        {
            return false;
        }

        _isEditing = true;
        _lastCommitError = string.Empty;
        LoadBufferFromSelected();
        return true;
    }

    private bool NavigateFromCurrentSelection(Func<bool> move)
    {
        if (_isEditing)
        {
            var finalized = FinalizeEditForNavigation();
            if (!finalized)
            {
                return true;
            }
        }

        return move();
    }

    private bool FinalizeEditForNavigation()
    {
        if (!_isEditing)
        {
            return true;
        }

        if (!CanEditCurrentField())
        {
            _isEditing = false;
            _isDirty = false;
            return true;
        }

        if (Model is null || SelectedField is null)
        {
            _isEditing = false;
            _isDirty = false;
            return true;
        }

        var currentValue = SafeReadValue(SelectedField, Model);
        if (!_isDirty && string.Equals(currentValue, _editBuffer, StringComparison.Ordinal))
        {
            _isEditing = false;
            _lastCommitError = string.Empty;
            return true;
        }

        _ = CommitCurrentField(out var success);
        return success;
    }

    private bool CommitCurrentField(out bool success)
    {
        success = false;
        if (!_isEditing || Model is null || SelectedField is null)
        {
            return false;
        }

        var previousValue = SafeReadValue(SelectedField, Model);
        if (!_isDirty && string.Equals(previousValue, _editBuffer, StringComparison.Ordinal))
        {
            _isEditing = false;
            _lastCommitError = string.Empty;
            success = true;
            return true;
        }

        var committedValue = _editBuffer;
        success = SelectedField.TryCommit(Model, committedValue, out var error);
        _lastCommitError = success ? string.Empty : (error ?? "Commit failed.");
        _isDirty = !success;

        if (success)
        {
            _editBuffer = SafeReadValue(SelectedField, Model);
            _isEditing = false;
        }

        FieldCommitted?.Invoke(
            this,
            new DataFormFieldCommittedEventArgs<TModel>(Model, _selectedIndex, SelectedField, previousValue, committedValue, success, _lastCommitError));
        return true;
    }

    private bool CancelCurrentEdit()
    {
        if (!_isEditing)
        {
            return false;
        }

        var previous = _editBuffer;
        LoadBufferFromSelected();
        _isEditing = false;
        return !string.Equals(previous, _editBuffer, StringComparison.Ordinal) || !string.IsNullOrEmpty(_lastCommitError);
    }

    private bool RemoveFromBuffer()
    {
        if (_editBuffer.Length == 0)
        {
            return false;
        }

        _editBuffer = _editBuffer[..^1];

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
}
