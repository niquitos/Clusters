using Clusters.Data.Models.Domain.Common;

namespace Clusters.Data.Models.Domain;

public record SortCriteria
{
    public Name FieldName { get;  }
    public bool Ascending { get;  }

    public SortCriteria(string fieldName, bool ascending)
    {
        FieldName = new Name(fieldName?.ToLowerInvariant());
        Ascending = ascending;
    }
}