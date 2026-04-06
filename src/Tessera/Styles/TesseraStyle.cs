using System.Collections.Concurrent;
using System.Text;

namespace Tessera.Styles;

/// <summary>
/// Represents an immutable ANSI style descriptor that can be composed and rendered to SGR escape sequences.
/// </summary>
/// <remarks>
/// Each nullable member uses tri-state semantics:
/// <c>true</c> enables a style flag, <c>false</c> emits the corresponding reset code, and <see langword="null" />
/// leaves the value unspecified so it can inherit during <see cref="Merge(TesseraStyle)" /> composition.
/// </remarks>
public readonly record struct TesseraStyle
{
    private const string ResetSequence = "\u001b[0m";
    private static readonly ConcurrentDictionary<TesseraStyle, RenderSequences> SequenceCache = new();

    private readonly record struct RenderSequences(string OpenSequence, string ResetSequence);

    /// <summary>
    /// Gets an empty style that does not emit any SGR parameters.
    /// </summary>
    public static TesseraStyle Empty => default;

    /// <summary>
    /// Gets a value indicating whether bold is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Bold { get; init; }
    /// <summary>
    /// Gets a value indicating whether dim is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Dim { get; init; }
    /// <summary>
    /// Gets a value indicating whether italic is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Italic { get; init; }
    /// <summary>
    /// Gets a value indicating whether underline is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Underline { get; init; }
    /// <summary>
    /// Gets a value indicating whether double underline is enabled, disabled, or unspecified.
    /// </summary>
    public bool? DoubleUnderline { get; init; }
    /// <summary>
    /// Gets a value indicating whether blink is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Blink { get; init; }
    /// <summary>
    /// Gets a value indicating whether strikethrough is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Strikethrough { get; init; }
    /// <summary>
    /// Gets a value indicating whether conceal is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Conceal { get; init; }
    /// <summary>
    /// Gets a value indicating whether overline is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Overline { get; init; }
    /// <summary>
    /// Gets a value indicating whether framed is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Framed { get; init; }
    /// <summary>
    /// Gets a value indicating whether encircled is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Encircled { get; init; }
    /// <summary>
    /// Gets a value indicating whether inverse is enabled, disabled, or unspecified.
    /// </summary>
    public bool? Inverse { get; init; }
    /// <summary>
    /// Gets the foreground color override when specified.
    /// </summary>
    public AnsiColor? Foreground { get; init; }
    /// <summary>
    /// Gets the background color override when specified.
    /// </summary>
    public AnsiColor? Background { get; init; }

    /// <summary>
    /// Returns a copy with <see cref="Bold" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable bold; otherwise disables bold.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithBold(bool enabled = true) => this with { Bold = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Dim" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable dim; otherwise disables dim.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithDim(bool enabled = true) => this with { Dim = enabled };
    /// <summary>
    /// Applies typography emphasis intent using ANSI SGR emphasis flags.
    /// </summary>
    /// <param name="weight">The typography emphasis intent to apply.</param>
    /// <remarks>
    /// This controls ANSI bold/dim emphasis only and does not control terminal font families, point sizes, or real font engines.
    /// </remarks>
    public TesseraStyle WithFontWeight(TesseraFontWeight weight)
    {
        return weight switch
        {
            TesseraFontWeight.Bold => this with { Bold = true, Dim = false },
            TesseraFontWeight.Dim => this with { Bold = false, Dim = true },
            _ => this with { Bold = false, Dim = false },
        };
    }

    /// <summary>
    /// Returns a copy with <see cref="Italic" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable italic; otherwise disables italic.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithItalic(bool enabled = true) => this with { Italic = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Underline" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable underline; otherwise disables underline.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithUnderline(bool enabled = true) => this with { Underline = enabled };
    /// <summary>
    /// Returns a copy with <see cref="DoubleUnderline" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable double underline; otherwise disables it.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithDoubleUnderline(bool enabled = true) => this with { DoubleUnderline = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Blink" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable blink; otherwise disables blink.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithBlink(bool enabled = true) => this with { Blink = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Strikethrough" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable strikethrough; otherwise disables it.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithStrikethrough(bool enabled = true) => this with { Strikethrough = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Conceal" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable conceal; otherwise disables conceal.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithConceal(bool enabled = true) => this with { Conceal = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Overline" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable overline; otherwise disables overline.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithOverline(bool enabled = true) => this with { Overline = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Framed" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable framed mode; otherwise disables it.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithFramed(bool enabled = true) => this with { Framed = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Encircled" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable encircled mode; otherwise disables it.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithEncircled(bool enabled = true) => this with { Encircled = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Inverse" /> set.
    /// </summary>
    /// <param name="enabled"><see langword="true" /> to enable inverse colors; otherwise disables inverse.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithInverse(bool enabled = true) => this with { Inverse = enabled };
    /// <summary>
    /// Returns a copy with <see cref="Foreground" /> set.
    /// </summary>
    /// <param name="color">The foreground color to apply.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithForeground(AnsiColor color) => this with { Foreground = color };
    /// <summary>
    /// Returns a copy with <see cref="Background" /> set.
    /// </summary>
    /// <param name="color">The background color to apply.</param>
    /// <returns>A new style value.</returns>
    public TesseraStyle WithBackground(AnsiColor color) => this with { Background = color };

    /// <summary>
    /// Composes this style with another style, where values in <paramref name="other" /> override specified members.
    /// </summary>
    /// <param name="other">The higher-priority style layer.</param>
    /// <returns>A composed style containing merged member values.</returns>
    /// <remarks>
    /// This method does not emit any escape codes. It only combines style descriptors:
    /// unspecified members in <paramref name="other" /> preserve the current value.
    /// </remarks>
    public TesseraStyle Merge(TesseraStyle other)
    {
        return new TesseraStyle
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

    /// <summary>
    /// Gets a value indicating whether this style has no configured members.
    /// </summary>
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

    /// <summary>
    /// Builds the opening ANSI SGR escape sequence for this style.
    /// </summary>
    /// <returns>The opening sequence, or an empty string when <see cref="IsEmpty" /> is <see langword="true" />.</returns>
    /// <remarks>
    /// This method returns only the opening sequence and does not include a trailing reset.
    /// </remarks>
    public string ToEscapeSequence()
    {
        if (IsEmpty)
        {
            return string.Empty;
        }

        return GetSequences().OpenSequence;
    }

    /// <summary>
    /// Wraps text with this style's opening SGR sequence and a reset sequence.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <returns>
    /// The styled text when both style and input are non-empty; otherwise the original <paramref name="text" />.
    /// </returns>
    /// <remarks>
    /// The trailing reset is <c>\u001b[0m</c>, so downstream output starts from a clean SGR state.
    /// </remarks>
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

    private static string BuildEscapeSequence(TesseraStyle style)
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
