using Tessera.Core.Abstractions;
using Tessera.Core.Input.Decoding;
using Tessera.Hosting;
using AnsiRendererOptions = Tessera.Core.Rendering.AnsiRendererOptions;
using TerminalCapabilityProfile = Tessera.Core.Terminal.Capabilities.TerminalCapabilityProfile;
using TerminalColorProfile = Tessera.Core.Terminal.TerminalColorProfile;
using TerminalSize = Tessera.Core.Terminal.TerminalSize;

namespace Tessera.Internal;

internal static class TesseraHostingAdapter
{
    public static AnsiRendererOptions ToCore(this Hosting.AnsiRendererOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new AnsiRendererOptions
        {
            FlushTimeout = options.FlushTimeout,
            QueryModeReports = options.QueryModeReports,
            QueryModeReportsOncePerMode = options.QueryModeReportsOncePerMode,
            IncludeKittyKeyboardBaseFlag = options.IncludeKittyKeyboardBaseFlag
        };
    }

    public static TerminalCapabilityProfile ToCore(this Hosting.TerminalCapabilityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return new TerminalCapabilityProfile(
            profile.FocusReporting,
            profile.MouseReporting,
            profile.BracketedPaste,
            profile.SynchronizedUpdates,
            profile.ModeReports,
            profile.SupportsOsc50FontRequests,
            profile.SupportsIterm2ProfileRequests,
            profile.Source);
    }

    public static TerminalColorProfile ToCore(this Hosting.TerminalColorProfile profile)
    {
        return profile switch
        {
            Hosting.TerminalColorProfile.Ansi16 => TerminalColorProfile.Ansi16,
            Hosting.TerminalColorProfile.Ansi256 => TerminalColorProfile.Ansi256,
            Hosting.TerminalColorProfile.TrueColor => TerminalColorProfile.TrueColor,
            _ => TerminalColorProfile.Unknown
        };
    }

    public static TerminalSize ToCore(this Hosting.TerminalSize size)
    {
        return new TerminalSize(size.Width, size.Height);
    }

    public static DecodeResult ToCore(this EventDecodeResult result)
    {
        return new DecodeResult(result.Consumed,
            result.Message is null ? null : TesseraMessageAdapter.ToCore(result.Message),
            result.NeedMoreData);
    }

    public static CursorStyle ToCore(this TerminalCursorStyle style)
    {
        return style switch
        {
            TerminalCursorStyle.BlinkingBlock => CursorStyle.BlinkingBlock,
            TerminalCursorStyle.SteadyBlock => CursorStyle.SteadyBlock,
            TerminalCursorStyle.BlinkingUnderline => CursorStyle.BlinkingUnderline,
            TerminalCursorStyle.SteadyUnderline => CursorStyle.SteadyUnderline,
            TerminalCursorStyle.BlinkingBar => CursorStyle.BlinkingBar,
            _ => CursorStyle.SteadyBar
        };
    }

    public static ScreenOutput ToCore(this RenderOutput output)
    {
        return new ScreenOutput(new ScreenFrame(output.Content)
        {
            CursorX = output.CursorX,
            CursorY = output.CursorY,
            CursorStyle = output.CursorStyle?.ToCore()
        })
        { Terminal = output.ScreenOptions.ToTerminalOutput() };
    }

    public static Hosting.TerminalCapabilityProfile AsHosting(this TerminalCapabilityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return new Hosting.TerminalCapabilityProfile(
            profile.FocusReporting,
            profile.MouseReporting,
            profile.BracketedPaste,
            profile.SynchronizedUpdates,
            profile.ModeReports,
            profile.SupportsOsc50FontRequests,
            profile.SupportsIterm2ProfileRequests,
            profile.Source);
    }

    public static Hosting.TerminalColorProfile AsHosting(this TerminalColorProfile profile)
    {
        return profile switch
        {
            TerminalColorProfile.Ansi16 => Hosting.TerminalColorProfile.Ansi16,
            TerminalColorProfile.Ansi256 => Hosting.TerminalColorProfile.Ansi256,
            TerminalColorProfile.TrueColor => Hosting.TerminalColorProfile.TrueColor,
            _ => Hosting.TerminalColorProfile.Unknown
        };
    }

    public static ValueTask<Hosting.TerminalSize> AsHosting(this ValueTask<TerminalSize> pending)
    {
        if (pending.IsCompletedSuccessfully)
        {
            var size = pending.Result;
            return new ValueTask<Hosting.TerminalSize>(size.AsHosting());
        }

        return AwaitSizeAsync(pending);
    }

    public static Hosting.TerminalSize AsHosting(this TerminalSize size)
    {
        return new Hosting.TerminalSize(size.Width, size.Height);
    }

    public static TerminalCursorStyle AsHosting(this CursorStyle style)
    {
        return style switch
        {
            CursorStyle.BlinkingBlock => TerminalCursorStyle.BlinkingBlock,
            CursorStyle.SteadyBlock => TerminalCursorStyle.SteadyBlock,
            CursorStyle.BlinkingUnderline => TerminalCursorStyle.BlinkingUnderline,
            CursorStyle.SteadyUnderline => TerminalCursorStyle.SteadyUnderline,
            CursorStyle.BlinkingBar => TerminalCursorStyle.BlinkingBar,
            _ => TerminalCursorStyle.SteadyBar
        };
    }

    public static EventDecodeResult ToHosting(this DecodeResult result)
    {
        return new EventDecodeResult(result.Consumed,
            result.Message is null ? null : TesseraMessageAdapter.ToPublic(result.Message),
            result.NeedMoreData);
    }

    public static RenderOutput ToHosting(this ScreenOutput output)
    {
        return new RenderOutput(output.Frame.Content)
        {
            CursorX = output.Frame.CursorX,
            CursorY = output.Frame.CursorY,
            CursorStyle = output.Frame.CursorStyle?.AsHosting(),
            ScreenOptions = output.Terminal.ToScreenOptions()
        };
    }

    private static async ValueTask<Hosting.TerminalSize> AwaitSizeAsync(ValueTask<TerminalSize> pending)
    {
        return (await pending.ConfigureAwait(false)).AsHosting();
    }
}
