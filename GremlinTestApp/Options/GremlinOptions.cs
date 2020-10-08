namespace GremlinTestApp.Options
{
    public class GremlinOptions
    {
        public const string APPSETTINGS_KEY = "Gremlin";

        public string? Host { get; set; }
        public string? PrimaryKey { get; set; }
        public string? Database { get; set; }
        public string? Container { get; set; }
        public int Port { get; set; }
    }
}
