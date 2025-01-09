namespace Saritasa.NetForge.Blazor.Domain.Exceptions;

/// <summary>
/// Entity Not Found Exception.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public NotFoundException() : base()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
