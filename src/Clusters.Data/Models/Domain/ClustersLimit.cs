using Clusters.Data.Models.Domain.Exceptions;

namespace Clusters.Data.Models.Domain;

public record ClustersLimit
{
    public const int MaxClusterLimit = 1000;
    
    public int Value { get;  }

    public ClustersLimit(int value)
    {
        if (value is < 1 or > MaxClusterLimit)
            throw new InvalidClustersLimitException();

        Value = value;
    }

    public static ClustersLimit Default => new(MaxClusterLimit);
}