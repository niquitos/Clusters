using Clusters.Data.Models;
using Clusters.Data.Models.Domain;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Clusters.Data.DataAccess;

public class DomainCsvTextDataReader
{
    private readonly CsvConfiguration _config;

    public DomainCsvTextDataReader()
    {
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            MissingFieldFound = null
        };
    }

    public List<ClusterEvent> ReadTextData(string filePath, int skip, int take)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, _config);

        csv.Context.RegisterClassMap<TextDataMap>();

        var events = csv.GetRecords<EventModel>().Skip(skip).Take(take);

        var jArray = new JArray();
        return MapToDomainEvents(events, jArray);
    }

    private static List<ClusterEvent> MapToDomainEvents(IEnumerable<EventModel> events, JArray jArray)
    {
        foreach (var @event in events)
        {
            var jObject = new JObject()
            {
                ["text"] = @event.Time,
                ["event_src.host"] = @event.EventSrcHost,
                ["alert.key"] = @event.AlertKey,
                ["correlation_name"] = @event.CorrelationName,
            };

            jArray.Add(jObject);
        }

        return [.. jArray.Select(j => new ClusterEvent(j))];
    }
}
