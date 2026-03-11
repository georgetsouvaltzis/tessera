using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.Text;
using NUnit.Framework;
using TeaSharp.TestFixtures;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;
using TeaSharp.Core.Messages;

namespace TeaSharp.IntegrationTests;

[TestFixture]
public sealed class DecoderDrivenWorkspaceUxTests
{
    [Test]
    public void CsiArrowUpSequenceIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001b[A");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void CsiArrowDownSequenceDecrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001b[B");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void Utf8QSequenceReturnsQuitCommand()
    {
        var model = new CounterFixtureModel();

        var result = ApplyDecoded(model, "q");

        Assert.That(result, Is.EqualTo(TeaSharp.Tea.Effects.Quit));
    }

    [Test]
    public void Ss3ArrowUpSequenceAlsoIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001bOA");

        Assert.That(model.Render().Frame.Content, Does.Contain("Count: 1"));
    }

    private static Effect? ApplyDecoded(CounterFixtureModel model, string sequence)
    {
        var decoder = new EventDecoder();
        var bytes = Encoding.UTF8.GetBytes(sequence);
        var index = 0;
        Effect? last = null;

        while (index < bytes.Length)
        {
            var result = decoder.Decode(bytes.AsSpan(index), timeoutExpired: false);
            if (result.Consumed == 0)
            {
                result = decoder.Decode(bytes.AsSpan(index), timeoutExpired: true);
            }

            if (result.Consumed == 0)
            {
                break;
            }

            index += result.Consumed;
            if (result.Message is not null)
            {
                last = model.Update(result.Message);
            }
        }

        return last;
    }
}
