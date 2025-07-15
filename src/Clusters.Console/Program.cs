using Clusters.Accord;
using Clusters.Clusterization;
using Clusters.Data.DataAccess;
using Clusters.Distances;
using Clusters.Hashing;
using Clusters.ML.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Clusters.Console;

internal class Program
{
    private static string Directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    private static string DataPath => Path.Combine(Directory, "Data\\sample-full.csv");

    private static void Main(string[] args)
    {
        //NaiveClusterizationMlNet();
        //TrigramClusterizationMlNet();
        //CustomFeaturesClusterizationMlNet();
        //NaiveClusterizationAccord();
        //GmmClusterizationAccord();
        //Fnv1aHashCollisions();
        //LevenshteinDistance();
        //DbscanClassic();
        DbscanBitArray();
        //System.Console.ReadKey();
    }

    private static void DbscanBitArray()
    {
        var reader = new CsvTextDataReader();

        var records = reader.ReadTextData(DataPath, 0, 30_000).ToList();

        Parallel.ForEach(records, @event =>
        {
            @event.SimHash = SimHashService.DoSimd(@event.Text!);
        });

        var stopwatch = Stopwatch.StartNew();
        DbscanBitArrayService.Clusterize([.. records]);
        stopwatch.Stop();

        System.Console.WriteLine($"Iteration: {1}, Clusterization took {(double)stopwatch.ElapsedMilliseconds / 1_000} s");


        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText(Path.Combine(Directory, "Data\\result.json"), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
    }

    private static void DbscanClassic()
    {
        var reader = new CsvTextDataReader();

        var records = reader.ReadTextData(DataPath, 0, 30_000).ToList();

        Parallel.ForEach(records, @event =>
        {
            @event.SimHash = SimHashService.DoSimd(@event.Text!);
        });

        var stopwatch = Stopwatch.StartNew();
        DbscanClassicService.Clusterize(records);
        stopwatch.Stop();

        System.Console.WriteLine($"Iteration: {1}, Clusterization took {(double)stopwatch.ElapsedMilliseconds / 1_000} s");


        var dictionary = records.GroupBy(x => x.ClusterId)
            .ToDictionary(g => g.Key, g => g.ToList())
            .OrderBy(x => x.Key);

        File.WriteAllText(Path.Combine(Directory, "Data\\result.json"), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
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
        System.Console.WriteLine($"Unique characters: {uniqueChars.Length}");
        System.Console.WriteLine($"Possible trigrams: {Math.Pow(uniqueChars.Length, 3):N0}");

        // Generate all possible trigrams
        var trigrams = GenerateAllTrigrams(uniqueChars);

        // Compute hashes and check for collisions
        var hashCounts = new Dictionary<ulong, List<string>>();
        int collisions = 0;

        foreach (var trigram in trigrams)
        {
            ulong hash = service.ComputeFnv1aUnsafeTrigramHash(trigram);

            if (!hashCounts.ContainsKey(hash))
            {
                hashCounts.Add(hash, []);
            }
            else
            {
                collisions++;
                System.Console.WriteLine($"Collision #{collisions}:");
                System.Console.WriteLine($"  '{trigram}' (hash: {hash:X16})");
                System.Console.WriteLine($"  Conflicts with: {string.Join(", ", hashCounts[hash])}");
            }

            hashCounts[hash].Add(trigram);
        }

        System.Console.WriteLine($"\nTotal collisions found: {collisions}");
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

        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistance(cat, cot));
        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized(cat, cot));
        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized_1(cat, cot));

        System.Console.WriteLine("------------");

        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistance(msg, msg2));
        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized(msg, msg2));
        System.Console.WriteLine(LevinshteinDistanceService.ComputeLevenshteinDistanceOptimized_1(msg, msg2));
    }
}
