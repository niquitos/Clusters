using System;

namespace Clusters.Data.Models.Domain.Common.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}