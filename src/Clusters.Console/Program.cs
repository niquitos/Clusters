using Clusters.Data.DataAccess;
using Clusters.ML.Net;
using Newtonsoft.Json;

namespace Clusters.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        var mlservice = new ClusteringService();
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData("Data\\sample-full.csv");

        mlservice.ClusterizeSingleFieldDefault(records);


        System.Console.WriteLine(JsonConvert.SerializeObject(records.OrderBy(x => x.ClusterId), Formatting.Indented));
    }
}
