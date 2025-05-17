using Clusters.Data.DataAccess;
using Clusters.ML.Net;
using Newtonsoft.Json;

namespace Clusters.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        //NaiveClusterizationMlNet();
        TrigramClusterizationMlNet();
    }

    private static void NaiveClusterizationMlNet()
    {
        var mlservice = new ClusterNaive();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData("Data\\sample-full.csv");

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
        var records = reader.ReadTextData("Data\\sample-full.csv");

        mlservice.ClusterizeSingleField10(records);

        var dictionary = records.GroupBy(x => x.ClusterId)
           .ToDictionary(g => g.Key, g => g.ToList())
           .OrderBy(x => x.Key);

        File.WriteAllText("Data\\result.json", JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }
}
