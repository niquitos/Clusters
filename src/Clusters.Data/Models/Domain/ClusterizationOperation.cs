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

    public void SetClusters(List<ClusterEvent> events)
    {
        var clusters = events
            .Where(x => x.ClusterId.HasValue && x.ClusterId != Guid.Empty)
            .GroupBy(x => x.ClusterId.Value)
            .ToDictionary(g => g.Key, g => g.ToList())
            .Select(x => CreateCluster(x.Key, x.Value));

        var sortedClusters = SortClusters(clusters);
        var namedClusters = SetClusterNames(sortedClusters);
        _clusters = SquashRemainingClusters(namedClusters).ToList();

        var garbageCluster = CreateCluster(Guid.Empty, [.. events.Where(x => x.ClusterId == Guid.Empty)]);
        garbageCluster.SetName(CreateClusterName(_clusters.Count + 1));

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

        return clustersArray.Take(ClustersLimit.MaxClusterLimit);
    }

    public Cluster CreateCluster(Guid clusterId, List<ClusterEvent> events)
    {
        var cluster = new Cluster(Id, clusterId);
        events.ForEach(x => x.SetClusterId(clusterId));
        cluster.SetEvents(events);
        return cluster;
    }

    private static string CreateClusterName(int count) => $"{count}";
}
