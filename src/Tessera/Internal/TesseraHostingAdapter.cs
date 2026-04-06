using Tessera.Core.Abstractions;
using Tessera.Core.Input;

namespace Tessera.Internal;

internal static class TesseraHostingAdapter
{
    public static global::Tessera.Core.Rendering.AnsiRendererOptions ToCore(this Hosting.AnsiRendererOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new global::Tessera.Core.Rendering.AnsiRendererOptions
        {
            FlushTimeout = options.FlushTimeout,
            QueryModeReports = options.QueryModeReports,
            QueryModeReportsOncePerMode = options.QueryModeReportsOncePerMode,
            IncludeKittyKeyboardBaseFlag = options.IncludeKittyKeyboardBaseFlag,
        };
    }

    public static global::Tessera.Core.Terminal.TerminalCapabilityProfile ToCore(this Hosting.TerminalCapabilityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return new global::Tessera.Core.Terminal.TerminalCapabilityProfile(
            profile.FocusReporting,
            profile.MouseReporting,
            profile.BracketedPaste,
            profile.SynchronizedUpdates,
            profile.ModeReports,
            profile.SupportsOsc50FontRequests,
            profile.SupportsIterm2ProfileRequests,
            profile.Source);
    }

    public static Hosting.TerminalCapabilityProfile AsHosting(this global::Tessera.Core.Terminal.TerminalCapabilityProfile profile)
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

    public static global::Tessera.Core.Terminal.TerminalColorProfile ToCore(this Hosting.TerminalColorProfile profile) =>
        profile switch
        {
            Hosting.TerminalColorProfile.Ansi16 => global::Tessera.Core.Terminal.TerminalColorProfile.Ansi16,
            Hosting.TerminalColorProfile.Ansi256 => global::Tessera.Core.Terminal.TerminalColorProfile.Ansi256,
            Hosting.TerminalColorProfile.TrueColor => global::Tessera.Core.Terminal.TerminalColorProfile.TrueColor,
            _ => global::Tessera.Core.Terminal.TerminalColorProfile.Unknown,
        };

    public static Hosting.TerminalColorProfile AsHosting(this global::Tessera.Core.Terminal.TerminalColorProfile profile) =>
        profile switch
        {
            global::Tessera.Core.Terminal.TerminalColorProfile.Ansi16 => Hosting.TerminalColorProfile.Ansi16,
            global::Tessera.Core.Terminal.TerminalColorProfile.Ansi256 => Hosting.TerminalColorProfile.Ansi256,
            global::Tessera.Core.Terminal.TerminalColorProfile.TrueColor => Hosting.TerminalColorProfile.TrueColor,
            _ => Hosting.TerminalColorProfile.Unknown,
        };

    public static global::Tessera.Core.Terminal.TerminalSize ToCore(this Hosting.TerminalSize size) =>
        new(size.Width, size.Height);

    public static ValueTask<Hosting.TerminalSize> AsHosting(this ValueTask<global::Tessera.Core.Terminal.TerminalSize> pending) =>
        pending.IsCompletedSuccessfully
            ? new ValueTask<Hosting.TerminalSize>(pending.Result.AsHosting())
            : AwaitSizeAsync(pending);

    public static Hosting.TerminalSize AsHosting(this global::Tessera.Core.Terminal.TerminalSize size) =>
        new(size.Width, size.Height);

    public static Hosting.EventDecodeResult ToHosting(this DecodeResult result) =>
        new(result.Consumed, result.Message is null ? null : TesseraMessageAdapter.ToPublic(result.Message), result.NeedMoreData);

    public static DecodeResult ToCore(this Hosting.EventDecodeResult result) =>
        new(result.Consumed, result.Message is null ? null : TesseraMessageAdapter.ToCore(result.Message), result.NeedMoreData);

    public static Hosting.RenderOutput ToHosting(this ScreenOutput output) =>
        new(output.Frame.Content)
        {
            CursorX = output.Frame.CursorX,
            CursorY = output.Frame.CursorY,
            CursorStyle = output.Frame.CursorStyle?.AsHosting(),
            ScreenOptions = output.Terminal.ToScreenOptions(),
        };

    public static ScreenOutput ToCore(this Hosting.RenderOutput output) =>
        new(new ScreenFrame(output.Content)
        {
            CursorX = output.CursorX,
            CursorY = output.CursorY,
            CursorStyle = output.CursorStyle?.ToCore(),
        })
        {
            Terminal = output.ScreenOptions.ToTerminalOutput(),
        };

    public static global::Tessera.Core.Abstractions.CursorStyle ToCore(this Hosting.TerminalCursorStyle style) =>
        style switch
        {
            Hosting.TerminalCursorStyle.BlinkingBlock => global::Tessera.Core.Abstractions.CursorStyle.BlinkingBlock,
            Hosting.TerminalCursorStyle.SteadyBlock => global::Tessera.Core.Abstractions.CursorStyle.SteadyBlock,
            Hosting.TerminalCursorStyle.BlinkingUnderline => global::Tessera.Core.Abstractions.CursorStyle.BlinkingUnderline,
            Hosting.TerminalCursorStyle.SteadyUnderline => global::Tessera.Core.Abstractions.CursorStyle.SteadyUnderline,
            Hosting.TerminalCursorStyle.BlinkingBar => global::Tessera.Core.Abstractions.CursorStyle.BlinkingBar,
            _ => global::Tessera.Core.Abstractions.CursorStyle.SteadyBar,
        };

    public static Hosting.TerminalCursorStyle AsHosting(this global::Tessera.Core.Abstractions.CursorStyle style) =>
        style switch
        {
            global::Tessera.Core.Abstractions.CursorStyle.BlinkingBlock => Hosting.TerminalCursorStyle.BlinkingBlock,
            global::Tessera.Core.Abstractions.CursorStyle.SteadyBlock => Hosting.TerminalCursorStyle.SteadyBlock,
            global::Tessera.Core.Abstractions.CursorStyle.BlinkingUnderline => Hosting.TerminalCursorStyle.BlinkingUnderline,
            global::Tessera.Core.Abstractions.CursorStyle.SteadyUnderline => Hosting.TerminalCursorStyle.SteadyUnderline,
            global::Tessera.Core.Abstractions.CursorStyle.BlinkingBar => Hosting.TerminalCursorStyle.BlinkingBar,
            _ => Hosting.TerminalCursorStyle.SteadyBar,
        };

    private static async ValueTask<Hosting.TerminalSize> AwaitSizeAsync(ValueTask<global::Tessera.Core.Terminal.TerminalSize> pending) =>
        (await pending.ConfigureAwait(false)).AsHosting();
}
