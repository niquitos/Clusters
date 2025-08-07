using Clusters.Data.Models.Domain.Common.Exceptions;

namespace Clusters.Data.Models.Domain.Exceptions;

public class ClusterizationException : DomainException
{
    public ClusterizationException(string message) : base(message) { }
}