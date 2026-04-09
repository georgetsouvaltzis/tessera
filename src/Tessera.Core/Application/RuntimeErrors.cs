namespace Tessera.Core.Application;

public sealed class TesseraRuntimeInterruptedException : Exception
{
    public TesseraRuntimeInterruptedException()
        : base("Runtime interrupted.")
    {
    }

    public TesseraRuntimeInterruptedException(string message)
        : base(message)
    {
    }

    public TesseraRuntimeInterruptedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
