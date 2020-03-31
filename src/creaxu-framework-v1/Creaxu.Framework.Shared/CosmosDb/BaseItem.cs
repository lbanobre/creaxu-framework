using System;
using Newtonsoft.Json;

namespace Creaxu.Framework.Shared.CosmosDb
{
    public class BaseItem
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "pk")]
        public string PartitionKey { get; set; }


        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
