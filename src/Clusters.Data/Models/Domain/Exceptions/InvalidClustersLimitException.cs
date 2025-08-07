namespace Clusters.Data.Models.Domain.Exceptions;

public class InvalidClustersLimitException : ClusterizationException
{
    public InvalidClustersLimitException() : base("Invalid number of clusters")
    {
    }
}