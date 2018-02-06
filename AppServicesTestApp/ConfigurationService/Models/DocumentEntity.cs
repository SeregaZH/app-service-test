using System;
using Newtonsoft.Json;

namespace ConfigurationService.Models
{
    public class DocumentEntity
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
    }
}
