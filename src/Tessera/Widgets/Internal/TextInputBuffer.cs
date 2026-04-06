namespace Tessera.Widgets.Internal;

internal static class TextInputBuffer
{
    public static TextInputBufferState SetValue(TextInputBufferState state, string value, int maxLength)
    {
        var nextValue = value.Length <= maxLength
            ? value
            : value[..maxLength];
        return new TextInputBufferState(nextValue, Math.Clamp(state.Cursor, 0, nextValue.Length), null);
    }

    public static (TextInputBufferState State, bool Changed) InsertText(TextInputBufferState state, string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
        {
            return (state, false);
        }

        state = DeleteSelectionIfAny(state).State;

        var available = Math.Max(0, maxLength - state.Value.Length);
        if (available <= 0)
        {
            return (state, false);
        }

        if (text.Length > available)
        {
            text = text[..available];
        }

        var value = state.Value.Insert(state.Cursor, text);
        return (new TextInputBufferState(value, state.Cursor + text.Length, null), text.Length > 0);
    }

    public static (TextInputBufferState State, bool Changed) DeleteBackward(TextInputBufferState state)
    {
        var deletedSelection = DeleteSelectionIfAny(state);
        if (deletedSelection.Changed)
        {
            return deletedSelection;
        }

        if (state.Cursor <= 0 || state.Value.Length == 0)
        {
            return (state, false);
        }

        return (state with
        {
            Value = state.Value.Remove(state.Cursor - 1, 1),
            Cursor = state.Cursor - 1,
        }, true);
    }

    public static (TextInputBufferState State, bool Changed) DeleteForward(TextInputBufferState state)
    {
        var deletedSelection = DeleteSelectionIfAny(state);
        if (deletedSelection.Changed)
        {
            return deletedSelection;
        }

        if (state.Cursor >= state.Value.Length || state.Value.Length == 0)
        {
            return (state, false);
        }

        return (state with { Value = state.Value.Remove(state.Cursor, 1) }, true);
    }

    public static (TextInputBufferState State, bool Changed) DeleteWordBackward(TextInputBufferState state)
    {
        var deletedSelection = DeleteSelectionIfAny(state);
        if (deletedSelection.Changed)
        {
            return deletedSelection;
        }

        var start = TextInputSelection.FindWordBoundaryLeft(state.Value, state.Cursor);
        if (start == state.Cursor)
        {
            return (state, false);
        }

        return (state with
        {
            Value = state.Value.Remove(start, state.Cursor - start),
            Cursor = start,
        }, true);
    }

    public static (TextInputBufferState State, bool Changed) DeleteWordForward(TextInputBufferState state)
    {
        var deletedSelection = DeleteSelectionIfAny(state);
        if (deletedSelection.Changed)
        {
            return deletedSelection;
        }

        var end = TextInputSelection.FindWordBoundaryRight(state.Value, state.Cursor);
        if (end == state.Cursor)
        {
            return (state, false);
        }

        return (state with { Value = state.Value.Remove(state.Cursor, end - state.Cursor) }, true);
    }

    private static (TextInputBufferState State, bool Changed) DeleteSelectionIfAny(TextInputBufferState state)
    {
        if (!state.HasSelection)
        {
            return (state, false);
        }

        var (start, end) = TextInputSelection.Range(state);
        return (new TextInputBufferState(state.Value.Remove(start, end - start), start, null), true);
    }
}
