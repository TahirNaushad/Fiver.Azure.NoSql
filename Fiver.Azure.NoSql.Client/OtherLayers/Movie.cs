using Newtonsoft.Json;

namespace Fiver.Azure.NoSql.Client.OtherLayers
{
    public class Movie
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Summary { get; set; }
    }
}
