namespace Clusters.Accord;

public static class FeaturizationService
{
    public static double[] FeaturizeText(string document, int vectorSize = 64)
    {
        var vector = new double[vectorSize];
        var trigrams = GetTrigrams(document);

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


