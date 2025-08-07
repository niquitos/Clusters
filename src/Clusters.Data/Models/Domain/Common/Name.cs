using Clusters.Data.Models.Domain.Common.Exceptions;

namespace Clusters.Data.Models.Domain.Common;

public record Name
{
    public string Value { get; }

    public Name(string value)
    {
        if (string.IsNullOrEmpty(value)) 
            throw new InvalidNameException("name value cannot be set to null or empty");
        
        Value = value;
    }
}