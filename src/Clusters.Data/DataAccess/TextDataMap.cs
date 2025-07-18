using Clusters.Data.Models;
using CsvHelper.Configuration;

namespace Clusters.Data.DataAccess;

public sealed class TextDataMap : ClassMap<EventModel>
{
    public TextDataMap()
    {
        Map(m => m.Time).Index(0).Optional();
        Map(m => m.EventSrcHost).Index(1).Optional();
        Map(m => m.Text).Index(2).Optional();
        Map(m => m.AlertKey).Index(3).Optional();
        Map(m => m.CorrelationName).Index(4).Optional();
    }
}
