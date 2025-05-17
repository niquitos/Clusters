using Microsoft.ML.Data;

namespace Clusters.ML.Net;

// Helper class for predictions
public class ClusterPrediction
{
    [ColumnName("PredictedLabel")]
    public uint PredictedClusterId { get; set; }
}
