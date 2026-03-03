namespace TeaSharp.Core.Application;

public sealed class TeaProgramInterruptedException : Exception
{
    public TeaProgramInterruptedException()
        : base("Program interrupted.")
    {
    }
}
