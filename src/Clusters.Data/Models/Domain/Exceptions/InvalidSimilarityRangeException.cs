namespace Clusters.Data.Models.Domain.Exceptions;

public class InvalidSimilarityRangeException : ClusterizationException
{
    public InvalidSimilarityRangeException() : base("the similarity value must be in range of 1-99")
    {
    }
}