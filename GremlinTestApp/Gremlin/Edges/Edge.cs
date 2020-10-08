using System;

namespace GremlinTestApp.Gremlin.Edges
{
    public abstract class Edge<T, Y, Z> : GraphItem
    {
        public Edge(T id, Y inVertex, Z outVertex)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            InVertex = inVertex ?? throw new ArgumentNullException(nameof(inVertex));
            OutVertex = outVertex ?? throw new ArgumentNullException(nameof(outVertex));
        }

        public T Id { get; set; }

        public Y InVertex { get; set; }

        public Z OutVertex { get; set; }
    }
}
