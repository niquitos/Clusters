using Accord.MachineLearning;
using Accord.Math;
using Clusters.Data.Models;

namespace Clusters.Accord;

public class ClusterMeanShiftAccordService
{
    public void ClusterizeSingleField(EventModel[] events)
    {
        Clusterize(events);
    }

    private void Clusterize(EventModel[] events)
    {
        var features = FeaturizeEvents(events);

        var meanShift = new MeanShift();

        var clusters = meanShift.Learn(features);
        var labels = clusters.Decide(features);

        for (var i = 0; i < events.Length; i++)
        {
            events[i].ClusterId = labels[i];
        }
    }
    private double[][] FeaturizeEvents(EventModel[] events)
    {
        return events.Select(x => FeaturizationService.FeaturizeText(x.Text)).ToArray();
    }
}


