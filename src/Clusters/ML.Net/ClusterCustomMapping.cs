using Clusters.Data.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

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
        
        var custom = CreateCustomTransformer(mlContext);

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

    private static CustomMappingEstimator<EventModel, OutputModel> CreateCustomTransformer(MLContext mlContext)
    {
        var schema = SchemaDefinition.Create(typeof(OutputModel));
        schema["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, 64);

        return mlContext.Transforms.CustomMapping<EventModel, OutputModel>(
            (input, output) =>output.Features = FeaturizeText(input.Text!),
            "customMapping",
            outputSchemaDefinition: schema);
    }

    private static float[] FeaturizeText(string text, int vectorSize = 64)
    {
        if (string.IsNullOrEmpty(text))
            return [];

        var vector = new float[vectorSize];

        for (int i = 0; i < text.Length - 2; i++)
        {
            var trigram = text.Substring(i, 3);
            var index = Math.Abs(trigram.GetHashCode()) % vectorSize;
            vector[index]++;
        }

        return vector;
    }
}

public class OutputModel
{
    public float[] Features { get; set; }
}
