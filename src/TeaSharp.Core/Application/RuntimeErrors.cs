namespace TeaSharp.Core.Application;

internal sealed class TeaRuntimeInterruptedException : Exception
{
    internal TeaRuntimeInterruptedException()
        : base("Runtime interrupted.")
    {
    }
}
