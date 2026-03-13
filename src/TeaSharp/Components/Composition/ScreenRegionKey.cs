using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Composition;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal readonly record struct ScreenRegionKey
{
    public ScreenRegionKey(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;

    public static ScreenRegionKey From(string value) => new(value);

    public static implicit operator ScreenRegionKey(string value) => new(value);
}
