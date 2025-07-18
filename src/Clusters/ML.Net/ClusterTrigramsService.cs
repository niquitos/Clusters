using Clusters.Data.Models;
using Microsoft.ML;

namespace Clusters.ML.Net;

public class ClusterTrigramsService
{
    public void ClusterizeSingleField10(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        ClusterWithKTriGrams(mlContext, events, 10);
    }

    public void ClusterizeSingleField20(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        ClusterWithKTriGrams(mlContext, events, 20);
    }

    public void ClusterizeSingleField30(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        ClusterWithKTriGrams(mlContext, events, 30);
    }

    private void ClusterWithKTriGrams(MLContext mlContext, EventModel[] events, int numberOfClusters)
    {
        var dataView = mlContext.Data.LoadFromEnumerable(events);

        var chars = mlContext.Transforms.Text.TokenizeIntoCharactersAsKeys(
            outputColumnName: "keys", 
            inputColumnName: "Text");

        var tokenization = mlContext.Transforms.Text.ProduceNgrams(
                outputColumnName: "ngrams",
                inputColumnName: "keys",
                ngramLength: 3,
                useAllLengths: false);

        var featurization = mlContext.Transforms.NormalizeLpNorm(
                outputColumnName: "features",
                inputColumnName: "ngrams");

        var clusterization = mlContext.Clustering.Trainers.KMeans(
                featureColumnName: "features",
                numberOfClusters: numberOfClusters);

        var pipeline = chars
            .Append(tokenization)
            .Append(featurization)
            .Append(clusterization);

        var model = pipeline.Fit(dataView);

        var predictions = mlContext.Data
            .CreateEnumerable<ClusterPrediction>(model.Transform(dataView), reuseRowObject: false)
            .Select(p => p.PredictedClusterId)
            .ToList();

        for (int i = 0; i < events.Length; i++)
        {
            events[i].ClusterId = predictions[i];
        }
    }
}