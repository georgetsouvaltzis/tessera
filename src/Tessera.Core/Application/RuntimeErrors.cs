namespace Tessera.Core.Application;

internal sealed class TesseraRuntimeInterruptedException : Exception
{
    internal TesseraRuntimeInterruptedException()
        : base("Runtime interrupted.")
    {
    }
}
