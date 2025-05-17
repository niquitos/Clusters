using Clusters.Data.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace Clusters.ML.Net;

public class ClusterCustomMapping
{
    public void ClusterizeSingleField10(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        Cluster(mlContext, events, 10);
    }

    public void ClusterizeSingleField20(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        Cluster(mlContext, events, 20);
    }

    public void ClusterizeSingleField30(EventModel[] events)
    {
        var mlContext = new MLContext(seed: 1);

        Cluster(mlContext, events, 30);
    }

    private void Cluster(MLContext mlContext, EventModel[] events, int numberOfClusters)
    {
        var dataView = mlContext.Data.LoadFromEnumerable(events);

        var schema = SchemaDefinition.Create(typeof(OutputModel));
        schema["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, 64);

        var custom = mlContext.Transforms.CustomMapping<EventModel, OutputModel>(
            (input, output) =>
            {
                output.Features = FeaturizeText(input.Text);
            }, 
            "customMapping", 
            outputSchemaDefinition: schema);

        var clusterization = mlContext.Clustering.Trainers.KMeans(
            new KMeansTrainer.Options
            {
                NumberOfClusters = numberOfClusters,
                FeatureColumnName = "Features"
            });

        var pipeline = custom.Append(clusterization);
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

    private static float[] FeaturizeText(string text, int vectorSize = 64)
    {
        var vector = new float[vectorSize];
        var trigrams = GetTrigrams(text);

        foreach (var trigram in trigrams)
        {
            int index = Math.Abs(trigram.GetHashCode()) % vectorSize;
            vector[index]++;
        }

        return vector;
    }

    private static string[] GetTrigrams(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var result = new string[text.Length - 2];

        for (int i = 0; i < text.Length - 2; i++)
        {
            result[i] = text.Substring(i, 3);
        }

        return result;
    }
}

public class OutputModel
{
    public float[] Features { get; set; }
}
