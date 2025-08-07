using Clusters.Data.Models.Domain.Common;

namespace Clusters.Data.Models.Domain;

public record FieldSimilarity
{
    public Name Name { get; }
    public Similarity Similarity { get; }

    public FieldSimilarity(string fieldName, double similarity)
    {
        Name = new Name(fieldName);
        Similarity = new Similarity(similarity);
    }
}