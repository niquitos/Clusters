namespace Clusters.Data.Models.Domain.Common.Exceptions;

public class InvalidNameException : DomainException
{
    public InvalidNameException(string message) : base(message)
    {
    }
}