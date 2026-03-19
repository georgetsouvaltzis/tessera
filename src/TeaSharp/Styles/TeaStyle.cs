using System.Collections.Concurrent;
using System.Text;

namespace TeaSharp.Styles;

public readonly record struct TeaStyle
{
    private const string ResetSequence = "\u001b[0m";
    private static readonly ConcurrentDictionary<TeaStyle, RenderSequences> SequenceCache = new();

    private readonly record struct RenderSequences(string OpenSequence, string ResetSequence);

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

        return GetSequences().OpenSequence;
    }

    public string Render(string text)
    {
        if (string.IsNullOrEmpty(text) || IsEmpty)
        {
            return text;
        }

        var sequences = GetSequences();
        return string.Concat(sequences.OpenSequence, text, sequences.ResetSequence);
    }

    private RenderSequences GetSequences()
    {
        return SequenceCache.GetOrAdd(
            this,
            static style => new RenderSequences(BuildEscapeSequence(style), ResetSequence));
    }

    private static string BuildEscapeSequence(TeaStyle style)
    {
        var builder = new StringBuilder(48);
        var hasAny = false;

        static void AppendParameter(StringBuilder builder, ref bool hasAny, string parameter)
        {
            if (hasAny)
            {
                builder.Append(';');
            }

            builder.Append(parameter);
            hasAny = true;
        }

        if (style.Bold is true)
        {
            AppendParameter(builder, ref hasAny, "1");
        }
        else if (style.Bold is false)
        {
            AppendParameter(builder, ref hasAny, "22");
        }

        if (style.Dim is true)
        {
            AppendParameter(builder, ref hasAny, "2");
        }
        else if (style.Dim is false)
        {
            AppendParameter(builder, ref hasAny, "22");
        }

        if (style.Italic is true)
        {
            AppendParameter(builder, ref hasAny, "3");
        }
        else if (style.Italic is false)
        {
            AppendParameter(builder, ref hasAny, "23");
        }

        if (style.DoubleUnderline is true)
        {
            AppendParameter(builder, ref hasAny, "21");
        }
        else if (style.Underline is true)
        {
            AppendParameter(builder, ref hasAny, "4");
        }

        if (style.Underline is false || (style.DoubleUnderline is false && style.Underline is not true))
        {
            AppendParameter(builder, ref hasAny, "24");
        }

        if (style.Blink is true)
        {
            AppendParameter(builder, ref hasAny, "5");
        }
        else if (style.Blink is false)
        {
            AppendParameter(builder, ref hasAny, "25");
        }

        if (style.Strikethrough is true)
        {
            AppendParameter(builder, ref hasAny, "9");
        }
        else if (style.Strikethrough is false)
        {
            AppendParameter(builder, ref hasAny, "29");
        }

        if (style.Conceal is true)
        {
            AppendParameter(builder, ref hasAny, "8");
        }
        else if (style.Conceal is false)
        {
            AppendParameter(builder, ref hasAny, "28");
        }

        if (style.Overline is true)
        {
            AppendParameter(builder, ref hasAny, "53");
        }
        else if (style.Overline is false)
        {
            AppendParameter(builder, ref hasAny, "55");
        }

        if (style.Encircled is true)
        {
            AppendParameter(builder, ref hasAny, "52");
        }
        else if (style.Framed is true)
        {
            AppendParameter(builder, ref hasAny, "51");
        }

        if ((style.Framed is false || style.Encircled is false) && style.Framed is not true && style.Encircled is not true)
        {
            AppendParameter(builder, ref hasAny, "54");
        }

        if (style.Inverse is true)
        {
            AppendParameter(builder, ref hasAny, "7");
        }
        else if (style.Inverse is false)
        {
            AppendParameter(builder, ref hasAny, "27");
        }

        if (style.Foreground is AnsiColor foreground)
        {
            AppendParameter(builder, ref hasAny, foreground.ToForegroundParameter());
        }

        if (style.Background is AnsiColor background)
        {
            AppendParameter(builder, ref hasAny, background.ToBackgroundParameter());
        }

        if (!hasAny)
        {
            return string.Empty;
        }

        return string.Concat("\u001b[", builder.ToString(), "m");
    }
}
