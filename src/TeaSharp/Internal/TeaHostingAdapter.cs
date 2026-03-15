using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Input;

namespace TeaSharp.Internal;

internal static class TeaHostingAdapter
{
    public static global::TeaSharp.Core.Rendering.AnsiRendererOptions ToCore(this Hosting.AnsiRendererOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new global::TeaSharp.Core.Rendering.AnsiRendererOptions
        {
            FlushTimeout = options.FlushTimeout,
            QueryModeReports = options.QueryModeReports,
            QueryModeReportsOncePerMode = options.QueryModeReportsOncePerMode,
            IncludeKittyKeyboardBaseFlag = options.IncludeKittyKeyboardBaseFlag,
        };
    }

    public static global::TeaSharp.Core.Terminal.TerminalCapabilityProfile ToCore(this Hosting.TerminalCapabilityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return new global::TeaSharp.Core.Terminal.TerminalCapabilityProfile(
            profile.FocusReporting,
            profile.MouseReporting,
            profile.BracketedPaste,
            profile.SynchronizedUpdates,
            profile.ModeReports,
            profile.Source);
    }

    public static Hosting.TerminalCapabilityProfile AsHosting(this global::TeaSharp.Core.Terminal.TerminalCapabilityProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return new Hosting.TerminalCapabilityProfile(
            profile.FocusReporting,
            profile.MouseReporting,
            profile.BracketedPaste,
            profile.SynchronizedUpdates,
            profile.ModeReports,
            profile.Source);
    }

    public static global::TeaSharp.Core.Terminal.TerminalColorProfile ToCore(this Hosting.TerminalColorProfile profile) =>
        profile switch
        {
            Hosting.TerminalColorProfile.Ansi16 => global::TeaSharp.Core.Terminal.TerminalColorProfile.Ansi16,
            Hosting.TerminalColorProfile.Ansi256 => global::TeaSharp.Core.Terminal.TerminalColorProfile.Ansi256,
            Hosting.TerminalColorProfile.TrueColor => global::TeaSharp.Core.Terminal.TerminalColorProfile.TrueColor,
            _ => global::TeaSharp.Core.Terminal.TerminalColorProfile.Unknown,
        };

    public static Hosting.TerminalColorProfile AsHosting(this global::TeaSharp.Core.Terminal.TerminalColorProfile profile) =>
        profile switch
        {
            global::TeaSharp.Core.Terminal.TerminalColorProfile.Ansi16 => Hosting.TerminalColorProfile.Ansi16,
            global::TeaSharp.Core.Terminal.TerminalColorProfile.Ansi256 => Hosting.TerminalColorProfile.Ansi256,
            global::TeaSharp.Core.Terminal.TerminalColorProfile.TrueColor => Hosting.TerminalColorProfile.TrueColor,
            _ => Hosting.TerminalColorProfile.Unknown,
        };

    public static global::TeaSharp.Core.Terminal.TerminalSize ToCore(this Hosting.TerminalSize size) =>
        new(size.Width, size.Height);

    public static ValueTask<Hosting.TerminalSize> AsHosting(this ValueTask<global::TeaSharp.Core.Terminal.TerminalSize> pending) =>
        pending.IsCompletedSuccessfully
            ? new ValueTask<Hosting.TerminalSize>(pending.Result.AsHosting())
            : AwaitSizeAsync(pending);

    public static Hosting.TerminalSize AsHosting(this global::TeaSharp.Core.Terminal.TerminalSize size) =>
        new(size.Width, size.Height);

    public static Hosting.EventDecodeResult ToHosting(this DecodeResult result) =>
        new(result.Consumed, result.Message is null ? null : TeaMessageAdapter.ToPublic(result.Message), result.NeedMoreData);

    public static DecodeResult ToCore(this Hosting.EventDecodeResult result) =>
        new(result.Consumed, result.Message is null ? null : TeaMessageAdapter.ToCore(result.Message), result.NeedMoreData);

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

    public static global::TeaSharp.Core.Abstractions.CursorStyle ToCore(this Hosting.TerminalCursorStyle style) =>
        style switch
        {
            Hosting.TerminalCursorStyle.BlinkingBlock => global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingBlock,
            Hosting.TerminalCursorStyle.SteadyBlock => global::TeaSharp.Core.Abstractions.CursorStyle.SteadyBlock,
            Hosting.TerminalCursorStyle.BlinkingUnderline => global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingUnderline,
            Hosting.TerminalCursorStyle.SteadyUnderline => global::TeaSharp.Core.Abstractions.CursorStyle.SteadyUnderline,
            Hosting.TerminalCursorStyle.BlinkingBar => global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingBar,
            _ => global::TeaSharp.Core.Abstractions.CursorStyle.SteadyBar,
        };

    public static Hosting.TerminalCursorStyle AsHosting(this global::TeaSharp.Core.Abstractions.CursorStyle style) =>
        style switch
        {
            global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingBlock => Hosting.TerminalCursorStyle.BlinkingBlock,
            global::TeaSharp.Core.Abstractions.CursorStyle.SteadyBlock => Hosting.TerminalCursorStyle.SteadyBlock,
            global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingUnderline => Hosting.TerminalCursorStyle.BlinkingUnderline,
            global::TeaSharp.Core.Abstractions.CursorStyle.SteadyUnderline => Hosting.TerminalCursorStyle.SteadyUnderline,
            global::TeaSharp.Core.Abstractions.CursorStyle.BlinkingBar => Hosting.TerminalCursorStyle.BlinkingBar,
            _ => Hosting.TerminalCursorStyle.SteadyBar,
        };

    private static async ValueTask<Hosting.TerminalSize> AwaitSizeAsync(ValueTask<global::TeaSharp.Core.Terminal.TerminalSize> pending) =>
        (await pending.ConfigureAwait(false)).AsHosting();
}
