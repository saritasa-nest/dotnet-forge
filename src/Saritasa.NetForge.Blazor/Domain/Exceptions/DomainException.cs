namespace Saritasa.NetForge.Blazor.Domain.Exceptions;

/// <summary>
/// The exception represents the business logic or validation exception.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public DomainException() : base()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DomainException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}
