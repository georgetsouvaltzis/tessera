namespace Tessera.Core.Application.Internal;

internal static class TesseraBackgroundLoops
{
    public static async Task AwaitAsync(Task commandLoop, Task? inputLoop, Task? resizeLoop)
    {
        try
        {
            await commandLoop.ConfigureAwait(false);
        }
        catch
        {
            // Shutdown is best-effort; command-loop failures are surfaced earlier.
        }

        if (inputLoop is not null)
        {
            try
            {
                await inputLoop.ConfigureAwait(false);
            }
            catch
            {
                // Input-loop teardown is best-effort during shutdown.
            }
        }

        if (resizeLoop is not null)
        {
            try
            {
                await resizeLoop.ConfigureAwait(false);
            }
            catch
            {
                // Resize-loop teardown is best-effort during shutdown.
            }
        }
    }
}
