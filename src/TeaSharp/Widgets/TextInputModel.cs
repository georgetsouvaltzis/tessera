using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Widgets;

public readonly record struct TextInputFrame(string Text, int CursorColumn, bool PlaceholderVisible);

public readonly record struct TextInputUpdateResult(bool Changed, bool Submitted);

public sealed class TextInputModel
{
    public string Value { get; private set; } = string.Empty;

    public int Cursor { get; private set; }

    public int? SelectionAnchor { get; private set; }

    public int MaxLength { get; set; } = 512;

    public string Placeholder { get; set; } = string.Empty;

    public bool MaskInput { get; set; }

    public char MaskCharacter { get; set; } = '*';

    public bool HasSelection => SelectionAnchor is int anchor && anchor != Cursor;

    public void SetValue(string value)
    {
        Value = value.Length <= MaxLength
            ? value
            : value[..MaxLength];
        Cursor = Math.Clamp(Cursor, 0, Value.Length);
        SelectionAnchor = null;
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
            var changed = InsertText(paste.Content);
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        if (message is not KeyPressMsg key)
        {
            return default;
        }

        if (keyMap.Submit.Matches(key))
        {
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
            MoveCursor(FindWordBoundaryLeft(), extendSelection);
            return default;
        }

        if (keyMap.WordRight.Matches(key))
        {
            MoveCursor(FindWordBoundaryRight(), extendSelection);
            return default;
        }

        if (keyMap.Left.Matches(key))
        {
            MoveCursor(Cursor - 1, extendSelection);
            return default;
        }

        if (keyMap.Right.Matches(key))
        {
            MoveCursor(Cursor + 1, extendSelection);
            return default;
        }

        if (keyMap.Home.Matches(key))
        {
            MoveCursor(0, extendSelection);
            return default;
        }

        if (keyMap.End.Matches(key))
        {
            MoveCursor(Value.Length, extendSelection);
            return default;
        }

        if (keyMap.DeleteWordBackward.Matches(key))
        {
            var changed = DeleteWordBackward();
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        if (keyMap.DeleteWordForward.Matches(key))
        {
            var changed = DeleteWordForward();
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        if (keyMap.DeleteBackward.Matches(key))
        {
            var changed = DeleteBackward();
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        if (keyMap.DeleteForward.Matches(key))
        {
            var changed = DeleteForward();
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        if (key.Code == KeyCode.Character
            && !string.IsNullOrEmpty(key.Text)
            && !key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            && !key.Modifiers.HasFlag(KeyModifiers.Alt)
            && !key.Modifiers.HasFlag(KeyModifiers.Meta))
        {
            var changed = InsertText(key.Text);
            return new TextInputUpdateResult(changed, Submitted: false);
        }

        return default;
    }

    public TextInputFrame BuildFrame(int width)
    {
        if (width <= 0)
        {
            return new TextInputFrame(string.Empty, 0, PlaceholderVisible: false);
        }

        var isPlaceholder = Value.Length == 0;
        var raw = isPlaceholder
            ? Placeholder
            : Value;
        var visible = MaskInput && !isPlaceholder
            ? new string(MaskCharacter, raw.Length)
            : raw;

        var cursor = isPlaceholder
            ? 0
            : Math.Clamp(Cursor, 0, visible.Length);

        var start = 0;
        if (cursor >= width)
        {
            start = cursor - width + 1;
        }
        else if (visible.Length > width)
        {
            start = Math.Max(0, visible.Length - width);
        }

        start = Math.Clamp(start, 0, Math.Max(0, visible.Length - 1));

        var text = start >= visible.Length
            ? string.Empty
            : visible.Substring(start, Math.Min(width, visible.Length - start));
        if (text.Length < width)
        {
            text = text.PadRight(width);
        }

        var cursorColumn = Math.Clamp(cursor - start, 0, Math.Max(0, width - 1));
        return new TextInputFrame(text, cursorColumn, isPlaceholder);
    }

    private void MoveCursor(int target, bool extendSelection)
    {
        var clamped = Math.Clamp(target, 0, Value.Length);
        if (extendSelection)
        {
            SelectionAnchor ??= Cursor;
        }
        else
        {
            SelectionAnchor = null;
        }

        Cursor = clamped;
    }

    private bool InsertText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        DeleteSelectionIfAny();

        var available = Math.Max(0, MaxLength - Value.Length);
        if (available <= 0)
        {
            return false;
        }

        if (text.Length > available)
        {
            text = text[..available];
        }

        Value = Value.Insert(Cursor, text);
        Cursor += text.Length;
        SelectionAnchor = null;
        return text.Length > 0;
    }

    private bool DeleteBackward()
    {
        if (DeleteSelectionIfAny())
        {
            return true;
        }

        if (Cursor <= 0 || Value.Length == 0)
        {
            return false;
        }

        Value = Value.Remove(Cursor - 1, 1);
        Cursor--;
        return true;
    }

    private bool DeleteForward()
    {
        if (DeleteSelectionIfAny())
        {
            return true;
        }

        if (Cursor >= Value.Length || Value.Length == 0)
        {
            return false;
        }

        Value = Value.Remove(Cursor, 1);
        return true;
    }

    private bool DeleteWordBackward()
    {
        if (DeleteSelectionIfAny())
        {
            return true;
        }

        var start = FindWordBoundaryLeft();
        if (start == Cursor)
        {
            return false;
        }

        var length = Cursor - start;
        Value = Value.Remove(start, length);
        Cursor = start;
        return true;
    }

    private bool DeleteWordForward()
    {
        if (DeleteSelectionIfAny())
        {
            return true;
        }

        var end = FindWordBoundaryRight();
        if (end == Cursor)
        {
            return false;
        }

        Value = Value.Remove(Cursor, end - Cursor);
        return true;
    }

    private bool DeleteSelectionIfAny()
    {
        if (!HasSelection)
        {
            return false;
        }

        var (start, end) = SelectionRange();
        Value = Value.Remove(start, end - start);
        Cursor = start;
        SelectionAnchor = null;
        return true;
    }

    private (int Start, int End) SelectionRange()
    {
        var anchor = SelectionAnchor ?? Cursor;
        return (Math.Min(anchor, Cursor), Math.Max(anchor, Cursor));
    }

    private int FindWordBoundaryLeft()
    {
        var i = Math.Clamp(Cursor, 0, Value.Length);
        while (i > 0 && !IsWordChar(Value[i - 1]))
        {
            i--;
        }

        while (i > 0 && IsWordChar(Value[i - 1]))
        {
            i--;
        }

        return i;
    }

    private int FindWordBoundaryRight()
    {
        var i = Math.Clamp(Cursor, 0, Value.Length);
        while (i < Value.Length && !IsWordChar(Value[i]))
        {
            i++;
        }

        while (i < Value.Length && IsWordChar(Value[i]))
        {
            i++;
        }

        return i;
    }

    private static bool IsWordChar(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }
}
