using GremlinTestApp.Extensions;
using GremlinTestApp.Gremlin.Vertices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GremlinTestApp.Gremlin.Vertex
{
    public class PersonVertex : Vertex<Guid, string?>
    {
        public PersonVertex(Guid id, string? pk) : base(id, pk)
        {
        }

        public string? Name { get; set; }

        public static async Task<PersonVertex> MapAsync(IDictionary<string, object> items, CancellationToken cancelToken = default)
        {
            var jsonDocument = await BuildJsonDocument(items, cancelToken);
            var source = jsonDocument.RootElement;
            var properties = source.GetProperty("properties");

            var person = new PersonVertex(
                source.GetProperty("id").GetGuid(),
                properties.GetProperty("pk").GetVertexPropertyValue<string>())
            {
                Name = properties.GetProperty("name").GetVertexPropertyValue<string>()
            };

            return person;
        }
    }
}
