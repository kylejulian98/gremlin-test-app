using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GremlinTestApp.Gremlin
{
    public abstract class GraphItem
    {
        protected static async Task<JsonDocument> BuildJsonDocument(IDictionary<string, object> items, CancellationToken cancelToken = default)
        {
            using var jsonStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(jsonStream, items, cancellationToken: cancelToken);
            jsonStream.Position = 0;

            return await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancelToken);
        }
    }
}
