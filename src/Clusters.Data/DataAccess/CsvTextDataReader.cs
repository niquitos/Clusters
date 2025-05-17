using Clusters.Data.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Clusters.Data.DataAccess;

public class CsvTextDataReader
{
    private readonly CsvConfiguration _config;

    public CsvTextDataReader()
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };
    }

    public EventModel[] ReadTextData(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _config);

        csv.Context.RegisterClassMap<TextDataMap>();

        return [.. csv.GetRecords<EventModel>()];
    }

    public EventModel[] ReadTextData(string filePath, int skip, int take)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _config);

        csv.Context.RegisterClassMap<TextDataMap>();

        return csv.GetRecords<EventModel>().Skip(skip).Take(take).ToArray();
    }

    public EventModel[] ReadTextData(string filePath, Func<EventModel, bool> predicate, int skip, int take)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _config);

        csv.Context.RegisterClassMap<TextDataMap>();

        return csv.GetRecords<EventModel>().Where(predicate).Skip(skip).Take(take).ToArray();
    }
}
