namespace TeaSharp.Styles;

public readonly record struct TeaStyle
{
    public static TeaStyle Empty => default;

    public bool? Bold { get; init; }
    public bool? Dim { get; init; }
    public bool? Italic { get; init; }
    public bool? Underline { get; init; }
    public bool? DoubleUnderline { get; init; }
    public bool? Blink { get; init; }
    public bool? Strikethrough { get; init; }
    public bool? Conceal { get; init; }
    public bool? Overline { get; init; }
    public bool? Framed { get; init; }
    public bool? Encircled { get; init; }
    public bool? Inverse { get; init; }
    public AnsiColor? Foreground { get; init; }
    public AnsiColor? Background { get; init; }

    public TeaStyle WithBold(bool enabled = true) => this with { Bold = enabled };
    public TeaStyle WithDim(bool enabled = true) => this with { Dim = enabled };
    public TeaStyle WithItalic(bool enabled = true) => this with { Italic = enabled };
    public TeaStyle WithUnderline(bool enabled = true) => this with { Underline = enabled };
    public TeaStyle WithDoubleUnderline(bool enabled = true) => this with { DoubleUnderline = enabled };
    public TeaStyle WithBlink(bool enabled = true) => this with { Blink = enabled };
    public TeaStyle WithStrikethrough(bool enabled = true) => this with { Strikethrough = enabled };
    public TeaStyle WithConceal(bool enabled = true) => this with { Conceal = enabled };
    public TeaStyle WithOverline(bool enabled = true) => this with { Overline = enabled };
    public TeaStyle WithFramed(bool enabled = true) => this with { Framed = enabled };
    public TeaStyle WithEncircled(bool enabled = true) => this with { Encircled = enabled };
    public TeaStyle WithInverse(bool enabled = true) => this with { Inverse = enabled };
    public TeaStyle WithForeground(AnsiColor color) => this with { Foreground = color };
    public TeaStyle WithBackground(AnsiColor color) => this with { Background = color };

    public TeaStyle Merge(TeaStyle other)
    {
        return new TeaStyle
        {
            Bold = other.Bold ?? Bold,
            Dim = other.Dim ?? Dim,
            Italic = other.Italic ?? Italic,
            Underline = other.Underline ?? Underline,
            DoubleUnderline = other.DoubleUnderline ?? DoubleUnderline,
            Blink = other.Blink ?? Blink,
            Strikethrough = other.Strikethrough ?? Strikethrough,
            Conceal = other.Conceal ?? Conceal,
            Overline = other.Overline ?? Overline,
            Framed = other.Framed ?? Framed,
            Encircled = other.Encircled ?? Encircled,
            Inverse = other.Inverse ?? Inverse,
            Foreground = other.Foreground ?? Foreground,
            Background = other.Background ?? Background,
        };
    }

    public bool IsEmpty =>
        Bold is null &&
        Dim is null &&
        Italic is null &&
        Underline is null &&
        DoubleUnderline is null &&
        Blink is null &&
        Strikethrough is null &&
        Conceal is null &&
        Overline is null &&
        Framed is null &&
        Encircled is null &&
        Inverse is null &&
        Foreground is null &&
        Background is null;

    public string ToEscapeSequence()
    {
        if (IsEmpty)
        {
            return string.Empty;
        }

        var parts = new List<string>(8);

        if (Bold is true)
        {
            parts.Add("1");
        }
        else if (Bold is false)
        {
            parts.Add("22");
        }

        if (Dim is true)
        {
            parts.Add("2");
        }
        else if (Dim is false)
        {
            parts.Add("22");
        }

        if (Italic is true)
        {
            parts.Add("3");
        }
        else if (Italic is false)
        {
            parts.Add("23");
        }

        if (DoubleUnderline is true)
        {
            parts.Add("21");
        }
        else if (Underline is true)
        {
            parts.Add("4");
        }

        if (Underline is false || (DoubleUnderline is false && Underline is not true))
        {
            parts.Add("24");
        }

        if (Blink is true)
        {
            parts.Add("5");
        }
        else if (Blink is false)
        {
            parts.Add("25");
        }

        if (Strikethrough is true)
        {
            parts.Add("9");
        }
        else if (Strikethrough is false)
        {
            parts.Add("29");
        }

        if (Conceal is true)
        {
            parts.Add("8");
        }
        else if (Conceal is false)
        {
            parts.Add("28");
        }

        if (Overline is true)
        {
            parts.Add("53");
        }
        else if (Overline is false)
        {
            parts.Add("55");
        }

        if (Encircled is true)
        {
            parts.Add("52");
        }
        else if (Framed is true)
        {
            parts.Add("51");
        }

        if ((Framed is false || Encircled is false) && Framed is not true && Encircled is not true)
        {
            parts.Add("54");
        }

        if (Inverse is true)
        {
            parts.Add("7");
        }
        else if (Inverse is false)
        {
            parts.Add("27");
        }

        if (Foreground is AnsiColor foreground)
        {
            parts.Add(foreground.ToForegroundParameter());
        }

        if (Background is AnsiColor background)
        {
            parts.Add(background.ToBackgroundParameter());
        }

        if (parts.Count == 0)
        {
            return string.Empty;
        }

        return $"\u001b[{string.Join(";", parts)}m";
    }

    public string Render(string text)
    {
        if (string.IsNullOrEmpty(text) || IsEmpty)
        {
            return text;
        }

        return $"{ToEscapeSequence()}{text}\u001b[0m";
    }
}
