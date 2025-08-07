namespace Clusters.Data.Models.Domain.Exceptions;

public class InvalidFieldsNumberException: ClusterizationException
{
    public InvalidFieldsNumberException(string message) : base(message)
    {
    }
}