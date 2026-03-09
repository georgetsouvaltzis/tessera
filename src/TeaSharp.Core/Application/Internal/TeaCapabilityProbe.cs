using TeaSharp.Core.Abstractions;
using System.Text;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

internal sealed class TeaCapabilityProbe
{
    private static readonly int[] DefaultCapabilityProbeModes = [1000, 1002, 1003, 1004, 1006, 2004, 2026];
    private static readonly int[] CapabilityProbeRepresentativeModes = [1004, 1006, 2004, 2026];
    private static readonly int[] LegacyMouseProbeModes = [1000, 1002, 1003];

    private CapabilityProbeState? _state;

    public async Task StartAsync(
        ITerminalAdapter? terminal,
        ProgramOptions options,
        TerminalCapabilityProfile runtimeCapabilities,
        Action<IMessage> send,
        CancellationToken token)
    {
        if (terminal is null
            || options.DisableInput
            || !options.EnableCapabilityProbe
            || !terminal.IsInputInteractive
            || !terminal.IsOutputInteractive
            || !runtimeCapabilities.ModeReports)
        {
            return;
        }

        var modes = options.CapabilityProbeModes is { Count: > 0 }
            ? options.CapabilityProbeModes
            : DefaultCapabilityProbeModes;
        if (modes.Count == 0)
        {
            return;
        }

        var probe = new CapabilityProbeState(Guid.NewGuid(), modes);
        _state = probe;
        await SendQueriesAsync(terminal, modes, token).ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(options.CapabilityProbeTimeout, token).ConfigureAwait(false);
                send(new CapabilityProbeTimeoutMsg(probe.Id));
            }
            catch (OperationCanceledException)
            {
            }
        }, CancellationToken.None);
    }

    public void Observe(ModeReportMsg report)
    {
        if (_state is null || !_state.PendingModes.Remove(report.Mode))
        {
            return;
        }

        _state.SawAnyResponse = true;
        if (TryClassifyModeReportState(report.State, out var supported, out _) && supported)
        {
            _state.SupportedModes.Add(report.Mode);
        }

        if (_state.PendingModes.Count == 0)
        {
            _state = null;
        }
    }

    public bool HandleTimeout(CapabilityProbeTimeoutMsg timeout, ref TerminalCapabilityProfile runtimeCapabilities, IProgramRenderer? renderer, Action<IMessage> send)
    {
        if (_state is null || _state.Id != timeout.ProbeId)
        {
            return false;
        }

        var sawAnyResponse = _state.SawAnyResponse;
        var unresolvedModes = _state.PendingModes.Where(IsCapabilityRepresentativeProbeMode).ToArray();
        var hasLegacyMouseSupport = _state.SupportedModes.Any(IsLegacyMouseProbeMode);
        _state = null;

        if (!sawAnyResponse)
        {
            if (!runtimeCapabilities.ModeReports)
            {
                return false;
            }

            var source = runtimeCapabilities.Source;
            if (!source.Contains("+probe-timeout", StringComparison.Ordinal))
            {
                source += "+probe-timeout";
            }

            runtimeCapabilities = runtimeCapabilities with
            {
                ModeReports = false,
                Source = source,
            };
            renderer?.UpdateCapabilities(runtimeCapabilities);
            send(new TerminalCapabilitiesMsg(runtimeCapabilities));
            return true;
        }

        var next = runtimeCapabilities;
        foreach (var unresolvedMode in unresolvedModes)
        {
            next = unresolvedMode switch
            {
                1004 => next with { FocusReporting = false },
                1006 when hasLegacyMouseSupport => next,
                1006 => next with { MouseReporting = false },
                2004 => next with { BracketedPaste = false },
                2026 => next with { SynchronizedUpdates = false },
                _ => next,
            };
        }

        if (next == runtimeCapabilities)
        {
            return false;
        }

        var nextSource = next.Source;
        if (!nextSource.Contains("+probe-partial-timeout", StringComparison.Ordinal))
        {
            nextSource += "+probe-partial-timeout";
        }

        runtimeCapabilities = next with { Source = nextSource };
        renderer?.UpdateCapabilities(runtimeCapabilities);
        send(new TerminalCapabilitiesMsg(runtimeCapabilities));
        return true;
    }

    public static bool TryApplyModeReport(TerminalCapabilityProfile current, ModeReportMsg report, out TerminalCapabilityProfile next)
    {
        next = current;
        if (!TryClassifyModeReportState(report.State, out var supported, out var enabled))
        {
            return false;
        }

        var isTrackedMode = report.Mode is 1000 or 1002 or 1003 or 1004 or 1006 or 2004 or 2026;
        if (!isTrackedMode)
        {
            return false;
        }

        if (report.Mode is 1000 or 1002 or 1003 && !supported)
        {
            return false;
        }

        var updated = report.Mode switch
        {
            1000 or 1002 or 1003 => current with { MouseReporting = true, ModeReports = true },
            1004 => current with { FocusReporting = supported, ModeReports = true },
            1006 => current with { MouseReporting = supported, ModeReports = true },
            2004 => current with { BracketedPaste = supported, ModeReports = true },
            2026 => current with { SynchronizedUpdates = supported, ModeReports = true },
            _ => current,
        };

        var source = updated.Source;
        if (!source.Contains("+mode-report", StringComparison.Ordinal))
        {
            source += "+mode-report";
        }

        if (!supported && !source.Contains("+mode-report-unsupported", StringComparison.Ordinal))
        {
            source += "+mode-report-unsupported";
        }
        else if (supported && !enabled && !source.Contains("+mode-report-reset", StringComparison.Ordinal))
        {
            source += "+mode-report-reset";
        }

        if (report.Mode is 1000 or 1002 or 1003
            && supported
            && !source.Contains("+mode-report-mouse-legacy", StringComparison.Ordinal))
        {
            source += "+mode-report-mouse-legacy";
        }

        next = updated with { Source = source };
        return next != current;
    }

    public static bool TryRefineColorProfile(TerminalColorProfile current, CapabilityMsg capability, out TerminalColorProfile next)
    {
        next = current;
        if (!string.Equals(capability.Name, "RGB", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(capability.Name, "Tc", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var value = (capability.Value ?? string.Empty).Trim();
        var enabled = value.Length == 0
            || value == "1"
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("on", StringComparison.OrdinalIgnoreCase);
        if (!enabled || current == TerminalColorProfile.TrueColor)
        {
            return false;
        }

        next = TerminalColorProfile.TrueColor;
        return true;
    }

    private static async Task SendQueriesAsync(ITerminalAdapter terminal, IReadOnlyList<int> modes, CancellationToken token)
    {
        if (modes.Count == 0)
        {
            return;
        }

        var sequence = new StringBuilder(modes.Count * 10);
        foreach (var mode in modes)
        {
            sequence.Append("\u001b[?");
            sequence.Append(mode);
            sequence.Append("$p");
        }

        try
        {
            var bytes = Encoding.ASCII.GetBytes(sequence.ToString());
            await terminal.Output.WriteAsync(bytes, token).ConfigureAwait(false);
            await terminal.Output.FlushAsync(token).ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private static bool TryClassifyModeReportState(ModeReportState state, out bool supported, out bool enabled)
    {
        supported = false;
        enabled = false;
        switch (state)
        {
            case ModeReportState.Unsupported:
                return true;
            case ModeReportState.Set:
            case ModeReportState.PermanentlySet:
                supported = true;
                enabled = true;
                return true;
            case ModeReportState.Reset:
            case ModeReportState.PermanentlyReset:
                supported = true;
                enabled = false;
                return true;
            default:
                return false;
        }
    }

    private static bool IsCapabilityRepresentativeProbeMode(int mode)
    {
        for (var i = 0; i < CapabilityProbeRepresentativeModes.Length; i++)
        {
            if (CapabilityProbeRepresentativeModes[i] == mode)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsLegacyMouseProbeMode(int mode)
    {
        for (var i = 0; i < LegacyMouseProbeModes.Length; i++)
        {
            if (LegacyMouseProbeModes[i] == mode)
            {
                return true;
            }
        }

        return false;
    }

    internal sealed class CapabilityProbeTimeoutMsg(Guid probeId) : IMessage
    {
        public Guid ProbeId { get; } = probeId;
    }

    private sealed class CapabilityProbeState
    {
        public CapabilityProbeState(Guid id, IReadOnlyList<int> modes)
        {
            Id = id;
            PendingModes = new HashSet<int>(modes);
            SupportedModes = new HashSet<int>();
        }

        public Guid Id { get; }

        public HashSet<int> PendingModes { get; }

        public HashSet<int> SupportedModes { get; }

        public bool SawAnyResponse { get; set; }
    }
}
