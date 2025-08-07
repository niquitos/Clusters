namespace Clusters.Data.Models.Domain.Common.Exceptions;

public class EventMissingFieldException : DomainException
{
    public EventMissingFieldException(string message) : base(message)
    {
    }
}