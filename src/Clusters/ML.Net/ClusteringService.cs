using Clusters.Data.Models;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace Clusters.ML.Net;

public class ClusteringService
{
    public void ClusterizeSingleFieldNaive10(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1); 

        ClusterWithK(mlContext, events, 10);
    }

    public void ClusterizeSingleFieldNaive20(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1); 

        ClusterWithK(mlContext, events, 20);
    }

    public void ClusterizeSingleFieldNaive30(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1); 

        ClusterWithK(mlContext, events, 30);
    }

    public void ClusterizeSingleFieldNaive500(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        ClusterWithK(mlContext, events, 500);
    }

    public void ClusterizeSingleFieldNaive1000(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        ClusterWithK(mlContext, events, 1000);
    }

    private void ClusterWithK(MLContext mlContext, EventModel[] events, int numberOfClusters)
    {
        var dataView = mlContext.Data.LoadFromEnumerable(events);

        var featurization = mlContext.Transforms.Text
            .FeaturizeText(inputColumnName: nameof(EventModel.Text), outputColumnName: "TextFeatures");

        var minMaxNormalization = mlContext.Transforms
            .NormalizeMinMax(inputColumnName: "TextFeatures", outputColumnName: "Features");

        var clusterization = mlContext.Clustering.Trainers.KMeans(
            new KMeansTrainer.Options
            {
                NumberOfClusters = numberOfClusters,
                FeatureColumnName = "Features"
            });

        var pipeline = featurization.Append(minMaxNormalization).Append(clusterization);
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