using System.Text;
using NUnit.Framework;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class DecoderDrivenWorkspaceUxTests
{
    [Test]
    public async Task GhosttyShiftSemicolonSequence_EntersCommandMode()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);

        ApplyDecoded(model, "3");
        ApplyDecoded(model, "\u001b[27;2;59~");

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=cmd"));
        Assert.That(view, Does.Contain("page=showcase"));
    }

    [Test]
    public async Task CsiUColonSequence_EntersCommandMode()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);

        ApplyDecoded(model, "3");
        ApplyDecoded(model, "\u001b[58;2u");

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=cmd"));
        Assert.That(view, Does.Contain("page=showcase"));
    }

    [Test]
    public async Task EscByteTimeout_DecodesAndExitsCommandMode()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);

        ApplyDecoded(model, "3");
        ApplyDecoded(model, "\u001b[58;2u");
        ApplyDecodedEscWithTimeout(model);

        var view = model.View().Content;
        Assert.That(view, Does.Contain("mode=nav"));
        Assert.That(view, Does.Contain("page=showcase"));
    }

    [Test]
    public async Task UppercaseP_FromUtf8AndCsiU_CyclesPaneBackward()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);

        ApplyDecoded(model, "3");
        ApplyDecoded(model, "\t");
        ApplyDecoded(model, "\u001b[58;2u");
        var before = ShowcasePaneToken(model.View().Content);

        ApplyDecoded(model, "P");
        var afterUtf8 = ShowcasePaneToken(model.View().Content);
        ApplyDecoded(model, "\u001b[80;2u");
        var afterCsiU = ShowcasePaneToken(model.View().Content);

        Assert.That(afterUtf8, Is.Not.EqualTo(before));
        Assert.That(afterCsiU, Is.Not.EqualTo(afterUtf8));
    }

    [Test]
    public async Task CommandModeAndShowcaseFocus_EnableTAndMHotkeys_FromDecodedBytes()
    {
        await using var terminal = new ConsoleTerminalAdapter();
        var model = new CounterModel(terminal);

        ApplyDecoded(model, "3");
        ApplyDecoded(model, "\t");
        ApplyDecoded(model, "t");
        ApplyDecoded(model, "m");
        Assert.That(ToastCount(model), Is.EqualTo(0));
        Assert.That(Modal(model).Visible, Is.False);

        ApplyDecoded(model, "\u001b[58;2u");
        ApplyDecoded(model, "t");
        ApplyDecoded(model, "m");
        Assert.That(ToastCount(model), Is.EqualTo(1));
        Assert.That(Modal(model).Visible, Is.True);
    }

    private static void ApplyDecoded(CounterModel model, string sequence)
    {
        var decoder = new EventDecoder();
        var bytes = Encoding.UTF8.GetBytes(sequence);
        var index = 0;
        var timeoutExpired = false;

        while (index < bytes.Length)
        {
            var result = decoder.Decode(bytes.AsSpan(index), timeoutExpired);
            if (result.Consumed == 0)
            {
                if (!result.NeedMoreData)
                {
                    break;
                }

                timeoutExpired = true;
                continue;
            }

            index += result.Consumed;
            timeoutExpired = false;
            if (result.Message is not null)
            {
                model.Update(result.Message);
            }
        }
    }

    private static void ApplyDecodedEscWithTimeout(CounterModel model)
    {
        var decoder = new EventDecoder();
        var result = decoder.Decode([0x1B], timeoutExpired: true);
        if (result.Message is not null)
        {
            model.Update(result.Message);
        }
    }

    private static string ShowcasePaneToken(string content)
    {
        const string marker = "pane=";
        var index = content.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0)
        {
            return string.Empty;
        }

        index += marker.Length;
        var end = index;
        while (end < content.Length && !char.IsWhiteSpace(content[end]))
        {
            end++;
        }

        return content[index..end];
    }

    private static ModalComponent Modal(CounterModel model)
    {
        var field = model.GetType().GetField("_showcaseModal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return field?.GetValue(model) as ModalComponent
            ?? throw new InvalidOperationException("CounterModel._showcaseModal missing.");
    }

    private static int ToastCount(CounterModel model)
    {
        var field = model.GetType().GetField("_showcaseToasts", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var center = field?.GetValue(model);
        if (center is null)
        {
            throw new InvalidOperationException("CounterModel._showcaseToasts missing.");
        }

        var toastsField = center.GetType().GetField("_toasts", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return toastsField?.GetValue(center) is System.Collections.ICollection collection
            ? collection.Count
            : 0;
    }
}
