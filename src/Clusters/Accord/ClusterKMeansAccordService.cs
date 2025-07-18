using Accord.MachineLearning;
using Accord.Math;
using Clusters.Data.Models;

namespace Clusters.Accord;

public class ClusterKMeansAccordService
{
    public void ClusterizeSingleField10(EventModel[] events)
    {
        Clusterize(events, 10);
    }

    public void ClusterizeSingleField20(EventModel[] events)
    {
        Clusterize(events, 20);
    }

    public void ClusterizeSingleField30(EventModel[] events)
    {
        Clusterize(events, 30);
    }

    public void Featurize(EventModel[] events)
    {
        FeaturizeEvents(events);
    }

    private void Clusterize(EventModel[] events, int numberOfClusters)
    {
        var features = FeaturizeEvents(events);

        var kmeans = new KMeans(numberOfClusters);

        var clusters = kmeans.Learn(features);
        int[] labels = clusters.Decide(features);

        for (int i = 0; i < events.Length; i++)
        {
            events[i].ClusterId = labels[i];
        }
    }
    private double[][] FeaturizeEvents(EventModel[] events)
    {
        return events.Select(x => FeaturizeText(x.Text)).ToArray();
    }

    private double[] FeaturizeText(string document, int vectorSize = 64)
    {
        var vector = new double[vectorSize];
        var trigrams = GetTrigrams(document);

        foreach (var trigram in trigrams)
        {
            int index = Math.Abs(trigram.GetHashCode()) % vectorSize;
            vector[index]++;
        }

        return vector;
    }

    private string[] GetTrigrams(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var result = new string[text.Length-2];

        for (int i = 0; i < text.Length-2; i++)
        {
            result[i] = text.Substring(i, 3);
        }

        return result;
    }
}


