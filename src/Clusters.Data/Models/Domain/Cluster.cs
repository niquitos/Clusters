using System;
using System.Collections.Generic;
using System.Linq;
using Clusters.Data.Models.Domain.Common;

namespace Clusters.Data.Models.Domain;

public class Cluster
{
    private List<ClusterEvent> _events;
    
    public Guid Id { get; }
    public Guid SessionId { get; }
    public Name Name { get; private set; }
    
    public IReadOnlyList<ClusterEvent> Events => _events;

    public Cluster(Guid sessionId, Guid id)
    {
        Id = id;
        SessionId = sessionId;
        _events = [];
    }

    public void SetEvents(List<ClusterEvent> events)
    {
        _events = events;
    }

    public void AddEvent(ClusterEvent @event)
    {
        @event.SetClusterId(Id);
        _events.Add(@event);
    }

    public void SetName(string name) => Name = new Name(name);

    public void SortEvents(SortCriteria sortCriteria)
    {
        var fieldName = sortCriteria.FieldName.Value;

        _events = sortCriteria.Ascending
            ? [.. _events.OrderBy(e => e.Payload[fieldName].ToString())]
            : [.. _events.OrderByDescending(e => e.Payload[fieldName].ToString())];
    }
}