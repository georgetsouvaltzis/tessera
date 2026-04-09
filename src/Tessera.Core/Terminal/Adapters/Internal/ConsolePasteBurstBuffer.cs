using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Terminal.Adapters.Internal;

internal sealed class ConsolePasteBurstBuffer
{
    private static readonly TimeSpan PasteBurstGap = TimeSpan.FromMilliseconds(28);
    private const int PasteBurstMinimumChars = 12;

    private readonly List<KeyPressMsg> _burst = new(64);
    private DateTimeOffset _lastBurstInputAt = DateTimeOffset.MinValue;

    public bool TryBuffer(IMessage? message, Action<IMessage> onEvent)
    {
        if (message is not KeyPressMsg keyPress || !IsPasteBurstCandidate(keyPress))
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow;
        if (_burst.Count > 0 && (now - _lastBurstInputAt) > PasteBurstGap)
        {
            Flush(onEvent);
        }

        _burst.Add(keyPress);
        _lastBurstInputAt = now;
        return true;
    }

    public void FlushIfIdle(Action<IMessage> onEvent)
    {
        if (_burst.Count == 0)
        {
            return;
        }

        if ((DateTimeOffset.UtcNow - _lastBurstInputAt) <= PasteBurstGap)
        {
            return;
        }

        Flush(onEvent);
        _lastBurstInputAt = DateTimeOffset.MinValue;
    }

    public void Flush(Action<IMessage> onEvent)
    {
        if (_burst.Count == 0)
        {
            return;
        }

        if (TryConvertBurstToPaste(_burst, out var paste))
        {
            onEvent(new PasteMsg(paste));
        }
        else
        {
            foreach (var key in _burst)
            {
                onEvent(key);
            }
        }

        _burst.Clear();
    }

    internal static bool TryConvertBurstToPaste(IReadOnlyList<KeyPressMsg> burst, out string content)
    {
        var sb = new StringBuilder();
        var hasLineBreak = false;
        var distinctChars = new HashSet<char>();

        foreach (var key in burst)
        {
            switch (key.Code)
            {
                case KeyCode.Character:
                    if (key.Text.Length > 0)
                    {
                        sb.Append(key.Text);
                        foreach (var ch in key.Text)
                        {
                            distinctChars.Add(ch);
                        }
                    }
                    break;
                case KeyCode.Enter:
                    sb.Append('\n');
                    hasLineBreak = true;
                    break;
                case KeyCode.Tab:
                    sb.Append('\t');
                    break;
            }
        }

        content = sb.ToString();
        if (content.Length == 0)
        {
            return false;
        }

        if (hasLineBreak && content.Length >= 2)
        {
            return true;
        }

        return content.Length >= PasteBurstMinimumChars && distinctChars.Count >= 2;
    }

    private static bool IsPasteBurstCandidate(KeyPressMsg key)
    {
        if (key.Modifiers != KeyModifiers.None)
        {
            return false;
        }

        return key.Code is KeyCode.Character or KeyCode.Enter or KeyCode.Tab;
    }
}
