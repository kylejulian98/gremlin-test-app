using System;

namespace GremlinTestApp.Gremlin.Vertices
{
    public abstract class Vertex<T, Y> : GraphItem
    {
        public Vertex(T id, Y pk)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Pk = pk ?? throw new ArgumentNullException(nameof(pk));
        }

        public T Id { get; set; }

        public Y Pk { get; set; }
    }
}