using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets.Internal;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TextInputModel
{
    public string Value { get; private set; } = string.Empty;

    public int Cursor { get; private set; }

    public int? SelectionAnchor { get; private set; }

    public int MaxLength { get; set; } = 512;

    public string Placeholder { get; set; } = string.Empty;

    public bool MaskInput { get; set; }

    public char MaskCharacter { get; set; } = '*';

    public bool Multiline { get; set; }

    public bool HasSelection => SelectionAnchor is int anchor && anchor != Cursor;

    public void SetValue(string value)
    {
        Apply(TextInputBuffer.SetValue(State, value, MaxLength));
    }

    public void Clear()
    {
        Value = string.Empty;
        Cursor = 0;
        SelectionAnchor = null;
    }

    public TextInputUpdateResult Update(IMessage message, TextInputKeyMap? keyMap = null)
    {
        keyMap ??= TextInputKeyMap.Default;

        if (message is PasteMsg paste)
        {
            return Apply(TextInputBuffer.InsertText(State, paste.Content, MaxLength));
        }

        if (message is not KeyPressMsg key)
        {
            return default;
        }

        if (keyMap.Submit.Matches(key))
        {
            if (Multiline
                && !key.Modifiers.HasFlag(KeyModifiers.Ctrl)
                && !key.Modifiers.HasFlag(KeyModifiers.Meta))
            {
                return Apply(TextInputBuffer.InsertText(State, "\n", MaxLength));
            }

            return new TextInputUpdateResult(Changed: false, Submitted: true);
        }

        if (keyMap.SelectAll.Matches(key))
        {
            SelectionAnchor = 0;
            Cursor = Value.Length;
            return new TextInputUpdateResult(Changed: false, Submitted: false);
        }

        var extendSelection = key.Modifiers.HasFlag(KeyModifiers.Shift);

        if (keyMap.WordLeft.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, TextInputSelection.FindWordBoundaryLeft(Value, Cursor), extendSelection));
            return default;
        }

        if (keyMap.WordRight.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, TextInputSelection.FindWordBoundaryRight(Value, Cursor), extendSelection));
            return default;
        }

        if (keyMap.Left.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, Cursor - 1, extendSelection));
            return default;
        }

        if (keyMap.Right.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, Cursor + 1, extendSelection));
            return default;
        }

        if (Multiline && key.Code == KeyCode.Up)
        {
            Apply(TextInputSelection.MoveCursor(State, TextInputSelection.MoveVerticalLine(Value, Cursor, -1), extendSelection));
            return default;
        }

        if (Multiline && key.Code == KeyCode.Down)
        {
            Apply(TextInputSelection.MoveCursor(State, TextInputSelection.MoveVerticalLine(Value, Cursor, 1), extendSelection));
            return default;
        }

        if (keyMap.Home.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, 0, extendSelection));
            return default;
        }

        if (keyMap.End.Matches(key))
        {
            Apply(TextInputSelection.MoveCursor(State, Value.Length, extendSelection));
            return default;
        }

        if (keyMap.DeleteWordBackward.Matches(key))
        {
            return Apply(TextInputBuffer.DeleteWordBackward(State));
        }

        if (keyMap.DeleteWordForward.Matches(key))
        {
            return Apply(TextInputBuffer.DeleteWordForward(State));
        }

        if (keyMap.DeleteBackward.Matches(key))
        {
            return Apply(TextInputBuffer.DeleteBackward(State));
        }

        if (keyMap.DeleteForward.Matches(key))
        {
            return Apply(TextInputBuffer.DeleteForward(State));
        }

        if (key.Code == KeyCode.Character
            && !string.IsNullOrEmpty(key.Text)
            && !key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            && !key.Modifiers.HasFlag(KeyModifiers.Alt)
            && !key.Modifiers.HasFlag(KeyModifiers.Meta))
        {
            return Apply(TextInputBuffer.InsertText(State, key.Text, MaxLength));
        }

        return default;
    }

    public TextInputFrame BuildFrame(int width)
    {
        return TextInputFrameBuilder.Build(Value, Placeholder, Multiline, MaskInput, MaskCharacter, Cursor, width);
    }

    private TextInputBufferState State => new(Value, Cursor, SelectionAnchor);

    private void Apply(TextInputBufferState state)
    {
        Value = state.Value;
        Cursor = state.Cursor;
        SelectionAnchor = state.SelectionAnchor;
    }

    private TextInputUpdateResult Apply((TextInputBufferState State, bool Changed) result)
    {
        Apply(result.State);
        return new TextInputUpdateResult(result.Changed, Submitted: false);
    }
}
