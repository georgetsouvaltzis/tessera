namespace Tessera.Core.Application;

/// <summary>
/// Thrown when the Tessera runtime loop is interrupted before completing normally.
/// </summary>
public sealed class TesseraRuntimeInterruptedException : Exception
{
    /// <summary>
    /// Initializes the exception with the default interruption message.
    /// </summary>
    public TesseraRuntimeInterruptedException()
        : base("Runtime interrupted.")
    {
    }

    /// <summary>
    /// Initializes the exception with a custom interruption message.
    /// </summary>
    /// <param name="message">The message that describes the interruption.</param>
    public TesseraRuntimeInterruptedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes the exception with a custom message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the interruption.</param>
    /// <param name="innerException">The underlying cause of the interruption.</param>
    public TesseraRuntimeInterruptedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
