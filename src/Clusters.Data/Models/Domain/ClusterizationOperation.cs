namespace Clusters.Data.Models.Domain;

public class ClusterizationOperation
{
    private List<Cluster> _clusters;

    public Guid Id { get; }
    public ClusterizationCriteria ClusterizationCriteria { get; }
    public SortCriteria[] EventsSortCriteria { get; }
    public ClustersLimit ClustersLimit { get; }
    public IReadOnlyList<Cluster> Clusters => _clusters;

    public ClusterizationOperation(ClusterizationCriteria clusterizationCriteria, SortCriteria[] eventsSortCriteria, ClustersLimit clustersLimit)
    {
        Id = Guid.NewGuid();
        ClusterizationCriteria = clusterizationCriteria;
        EventsSortCriteria = eventsSortCriteria ?? [];
        ClustersLimit = clustersLimit;
        _clusters = [];
    }

    public void AddCluster(Cluster cluster) => _clusters.Add(cluster);

    public void CreateClusters(List<ClusterEvent> events)
    {
        var clusters = events
            .Where(x => x.ClusterId.HasValue && x.ClusterId != Guid.Empty)
            .GroupBy(x => x.ClusterId.Value)
            .ToDictionary(g => g.Key, g => g.ToList())
            .Select(x => CreateCluster(x.Key, x.Value));

        var sortedClusters = SortClusters(clusters);
        var namedClusters = SetClusterNames(sortedClusters);
        _clusters = SquashRemainingClusters(namedClusters).ToList();

        var garbageEvents = events.Where(x => x.ClusterId == Guid.Empty).ToList();
        if (garbageEvents.Count == 0)
            return;

        var garbageCluster = CreateCluster(Guid.NewGuid(), garbageEvents);
        garbageCluster.SetName(CreateClusterName(_clusters.Count + 1));
        garbageCluster.SetClusterType(ClusterType.Garbage);

        _clusters.Add(garbageCluster);
    }

    private static IEnumerable<Cluster> SortClusters(IEnumerable<Cluster> clusters)
    {
        return clusters.OrderByDescending(c => c.Events.Count);
    }

    private static IEnumerable<Cluster> SetClusterNames(IEnumerable<Cluster> clusters)
    {
        var index = 1;
        foreach (var cluster in clusters)
        {
            cluster.SetName(CreateClusterName(index));
            index++;
            yield return cluster;
        }
    }

    private IEnumerable<Cluster> SquashRemainingClusters(IEnumerable<Cluster> clusters)
    {
        var clustersArray = clusters.ToList();
        if (clustersArray.Count <= ClustersLimit.MaxClusterLimit)
            return clustersArray;

        var events = clustersArray.Skip(ClustersLimit.MaxClusterLimit - 1).SelectMany(cluster => cluster.Events).ToList();

        var squashCluster = CreateCluster(Guid.NewGuid(), events);

        clustersArray[ClustersLimit.MaxClusterLimit - 1] = squashCluster;

        squashCluster.SetName(CreateClusterName(ClustersLimit.Value));
        squashCluster.SetClusterType(ClusterType.Merged);

        return clustersArray.Take(ClustersLimit.MaxClusterLimit);
    }

    public Cluster CreateCluster(Guid clusterId, List<ClusterEvent> events)
    {
        var cluster = new Cluster(clusterId, Id);
        events.ForEach(x => x.SetClusterId(clusterId));
        cluster.SetEvents(events);
        return cluster;
    }

    private static string CreateClusterName(int count) => $"{count}";
}
