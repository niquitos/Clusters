using Clusters.Data.Models.Domain.Exceptions;

namespace Clusters.Data.Models.Domain;

public record ClusterizationCriteria
{
    public FieldSimilarity[] Fields { get; }
    
    public ClusterizationCriteria(FieldSimilarity[] fields)
    {
        if (fields.Length is < 1 or > 3)
            throw new InvalidFieldsNumberException("the number of fields for clusteriazation must be in range of 1-3");
        
        Fields = fields;
    }
}