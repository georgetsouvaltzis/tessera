namespace Tessera.Widgets.Internal;

internal readonly record struct TextInputBufferState(string Value, int Cursor, int? SelectionAnchor)
{
    public bool HasSelection => SelectionAnchor is int anchor && anchor != Cursor;
}
