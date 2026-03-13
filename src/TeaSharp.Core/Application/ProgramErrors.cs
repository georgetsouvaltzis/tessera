namespace TeaSharp.Core.Application;

internal sealed class TeaProgramInterruptedException : Exception
{
    internal TeaProgramInterruptedException()
        : base("Program interrupted.")
    {
    }
}
