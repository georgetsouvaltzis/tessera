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

        Assert.That(model.View().Frame.Content, Does.Contain("Count: 1"));
    }

    [Test]
    public void CsiArrowDownSequenceDecrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001b[B");

        Assert.That(model.View().Frame.Content, Does.Contain("Count: -1"));
    }

    [Test]
    public void Utf8QSequenceReturnsQuitCommand()
    {
        var model = new CounterFixtureModel();

        var result = ApplyDecoded(model, "q");

        Assert.That(result, Is.EqualTo(TeaSharp.Tea.Cmd.Quit));
    }

    [Test]
    public void Ss3ArrowUpSequenceAlsoIncrementsCounter()
    {
        var model = new CounterFixtureModel();

        ApplyDecoded(model, "\u001bOA");

        Assert.That(model.View().Frame.Content, Does.Contain("Count: 1"));
    }

    private static Command? ApplyDecoded(CounterFixtureModel model, string sequence)
    {
        var decoder = new EventDecoder();
        var bytes = Encoding.UTF8.GetBytes(sequence);
        var index = 0;
        Command? last = null;

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
