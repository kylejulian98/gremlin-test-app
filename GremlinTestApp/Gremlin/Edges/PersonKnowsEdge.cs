using GremlinTestApp.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GremlinTestApp.Gremlin.Edges
{
    public class PersonKnowsEdge : Edge<Guid, Guid, Guid>
    {
        public PersonKnowsEdge(Guid id, Guid inVertex, Guid outVertex) 
            : base (id, inVertex, outVertex)
        {
        }

        public DateTime KnownSince { get; set; }

        public static async Task<PersonKnowsEdge> MapAsync(IDictionary<string, object> items, CancellationToken cancelToken = default)
        {
            var jsonDocument = await BuildJsonDocument(items, cancelToken);
            var source = jsonDocument.RootElement;
            var properties = source.GetProperty("properties");

            var person = new PersonKnowsEdge(
                source.GetProperty("id").GetGuid(),
                source.GetProperty("inV").GetGuid(),
                source.GetProperty("outV").GetGuid())
            {
                KnownSince = properties.TryGetProperty("knownSince", out var knownSinceProperty) ? knownSinceProperty.GetEdgePropertyValue<DateTime>() : default
            };

            return person;
        }
    }
}
