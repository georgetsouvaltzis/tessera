namespace TeaSharp.Core.Application;

internal static class TeaProgramFramePacer
{
    public static async Task<(bool Rendered, DateTimeOffset LastRender, bool PendingRender)> TryRenderAsync(
        bool adaptiveFramePacing,
        TimeSpan minFrame,
        DateTimeOffset lastRender,
        bool pendingRender,
        Func<Task> render,
        CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var elapsed = now - lastRender;
        if (!adaptiveFramePacing)
        {
            if (elapsed < minFrame)
            {
                await Task.Delay(minFrame - elapsed, token).ConfigureAwait(false);
            }

            await render().ConfigureAwait(false);
            return (true, DateTimeOffset.UtcNow, false);
        }

        if (elapsed >= minFrame)
        {
            await render().ConfigureAwait(false);
            return (true, DateTimeOffset.UtcNow, false);
        }

        return (false, lastRender, pendingRender);
    }

    public static async Task<(DateTimeOffset LastRender, bool PendingRender)> DelayAndRenderAsync(
        TimeSpan minFrame,
        DateTimeOffset lastRender,
        Func<Task> render,
        CancellationToken token)
    {
        var now = DateTimeOffset.UtcNow;
        var elapsed = now - lastRender;
        if (elapsed < minFrame)
        {
            await Task.Delay(minFrame - elapsed, token).ConfigureAwait(false);
        }

        await render().ConfigureAwait(false);
        return (DateTimeOffset.UtcNow, false);
    }
}
