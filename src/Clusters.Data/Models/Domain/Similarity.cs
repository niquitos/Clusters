using Clusters.Data.Models.Domain.Exceptions;

namespace Clusters.Data.Models.Domain;

public record Similarity
{
    public double Value { get; }

    public Similarity(double value)
    {
        if (value is < 1 or > 99) 
            throw new InvalidSimilarityRangeException();
        
        Value = value;
    }
}