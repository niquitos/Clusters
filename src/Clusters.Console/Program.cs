using Clusters.Accord;
using Clusters.Data.DataAccess;
using Clusters.ML.Net;
using Newtonsoft.Json;

namespace Clusters.Console;

internal class Program
{
    private const string DataPath = "Data\\sample-full.csv";
    private static void Main(string[] args)
    {
        //NaiveClusterizationMlNet();
        //TrigramClusterizationMlNet();
        //CustomFeaturesClusterizationMlNet();
        //NaiveClusterizationAccord();
        GmmClusterizationAccord();
    }

    private static void NaiveClusterizationMlNet()
    {
        var mlservice = new ClusterNaive();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath);

        mlservice.ClusterizeSingleField10(records);


        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void TrigramClusterizationMlNet()
    {
        var mlservice = new ClusterTrigramsService();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath);

        mlservice.ClusterizeSingleField10(records);

        var dictionary = records.GroupBy(x => x.ClusterId)
           .ToDictionary(g => g.Key, g => g.ToList())
           .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void CustomFeaturesClusterizationMlNet()
    {
        var mlservice = new ClusterCustomMapping();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath);

        mlservice.ClusterizeSingleField10(records);

        var dictionary = records.GroupBy(x => x.ClusterId)
           .ToDictionary(g => g.Key, g => g.ToList())
           .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void KMeansClusterizationAccord()
    {
        var mlservice = new ClusterKMeansAccordService();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 30000);

        mlservice.ClusterizeSingleField10(records);

        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void GmmClusterizationAccord()
    {
        var mlservice = new ClusterMeanShiftAccordService();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 30000);

        mlservice.ClusterizeSingleField(records);

        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }
}
