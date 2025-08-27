using Clusters.Accord;
using Clusters.Clusterization;
using Clusters.Data.DataAccess;
using Clusters.Data.Models.Domain;
using Clusters.Data.Models.Domain.Services;
using Clusters.Distances;
using Clusters.Hashing;
using Clusters.ML.Net;
using Newtonsoft.Json;
using System.Reflection;
using static System.Console;

namespace Clusters.Console;

internal class Program
{
    private static string Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    private static string DataPath => Path.Combine(Directory, "Data\\sample-100.csv");

    private static void Main(string[] args)
    {
        //NaiveClusterizationMlNet();
        //TrigramClusterizationMlNet();
        //CustomFeaturesClusterizationMlNet();
        //NaiveClusterizationAccord();
        //GmmClusterizationAccord();
        //Fnv1aHashCollisions();
        //LevenshteinDistance();
        RunDbScanClassic();
        //RunDbScanTest();
        SimHash();

        //Split2();
        DomainTests();

       //Trigramhashes();
    }

    private static void Trigramhashes()
    {
        var services = new TrigramHashingService();

        var input = "abc";

        WriteLine(services.Fnv1aHashSimpleFor(input));
        WriteLine(services.Fnv1aHashUnsafe(input));
        WriteLine(services.Fnv1aHashUnrolled(input));
        WriteLine(services.Fnv1aHashSimd(input));
    }

    private static unsafe  void DomainTests()
    {
        var operation = new ClusterizationOperation(
            new ClusterizationCriteria(
                [
                new FieldSimilarity("text", 80),
             //new FieldSimilarity("alert.key", 60),
             //new FieldSimilarity("correlation_name", 85)
            ]),
            [new SortCriteria("text", true)],
            new ClustersLimit(1000));

        var reader = new DomainCsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 10_000).ToList();

        var service = new ClusterizationService(operation);

        service.Clusterize(records);

        File.WriteAllText(Path.Combine(Directory, "Data\\result-domain.json"), JsonConvert.SerializeObject(operation.Clusters, Formatting.Indented));

    }

    private static void Split2()
    {
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 30_000).ToList();

        DbscanSplit2.Clusterize([.. records]);

        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText(Path.Combine(Directory, "Data\\result-100.json"), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void SimHash()
    {

        WriteLine(SimHashService.DoSimple(StringHelper.AllSymbolsInput));
        WriteLine(SimHashService.DoBitHack(StringHelper.AllSymbolsInput));
        WriteLine(SimHashService.DoUnsafe(StringHelper.AllSymbolsInput));
        WriteLine(SimHashService.DoUnrolled(StringHelper.AllSymbolsInput));
        WriteLine(SimHashService.DoSIMD(StringHelper.AllSymbolsInput));
        WriteLine(HashService.DoSIMD(StringHelper.AllSymbolsInput));
    }

    private static void RunDbScanTest()
    {
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 30_000).ToList();

        DbscanTest.Clusterize([.. records]);

        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText(Path.Combine(Directory, "Data\\result-test.json"), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void RunDbScanClassic()
    {
        var reader = new CsvTextDataReader();
        var records = reader.ReadTextData(DataPath, 0, 10_000).ToList();

        DbscanClassic.Clusterize([.. records]);

        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText(Path.Combine(Directory, "Data\\result-classic.json"), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
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

    private static void Fnv1aHashCollisions()
    {
        const string Characters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
        "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
        "0123456789" +
        "!@#$%^&*()_+-=[]{}|;:'\",.<>/?" +
        "\n\r\t\0";

        var service = new TrigramHashingService();

        var uniqueChars = Characters.Distinct().ToArray();
        WriteLine($"Unique characters: {uniqueChars.Length}");
        WriteLine($"Possible trigrams: {Math.Pow(uniqueChars.Length, 3):N0}");

        // Generate all possible trigrams
        var trigrams = GenerateAllTrigrams(uniqueChars);

        // Compute hashes and check for collisions
        var hashCounts = new Dictionary<ulong, List<string>>();
        int collisions = 0;

        foreach (var trigram in trigrams)
        {
            ulong hash = service.Fnv1aHashUnrolled(trigram);

            if (!hashCounts.ContainsKey(hash))
            {
                hashCounts.Add(hash, []);
            }
            else
            {
                collisions++;
                WriteLine($"Collision #{collisions}:");
                WriteLine($"  '{trigram}' (hash: {hash:X16})");
                WriteLine($"  Conflicts with: {string.Join(", ", hashCounts[hash])}");
            }

            hashCounts[hash].Add(trigram);
        }

        WriteLine($"\nTotal collisions found: {collisions}");
    }

    private static IEnumerable<string> GenerateAllTrigrams(char[] chars)
    {
        for (int i = 0; i < chars.Length; i++)
            for (int j = 0; j < chars.Length; j++)
                for (int k = 0; k < chars.Length; k++)
                    yield return new string(new[] { chars[i], chars[j], chars[k] });
    }

    private static void LevenshteinDistance()
    {
        const string cat = "cat";
        const string cot = "cot";

        const string msg = "brown fox jumps over the dog";
        const string msg2 = "how far the path could go";

        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistance(cat, cot));
        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized(cat, cot));
        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized_1(cat, cot));

        WriteLine("------------");

        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistance(msg, msg2));
        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized(msg, msg2));
        WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized_1(msg, msg2));
    }
}
