namespace TeaSharp.Components.Composition;

public readonly record struct ScreenRegionKey
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
