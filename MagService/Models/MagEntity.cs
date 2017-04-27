#region

using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace MagService.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MagEntity
    {
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "RId")]
        public List<long> ReferenceIds { get; set; }

        [JsonProperty(PropertyName = "F")]
        public List<Field> Fields { get; set; }

        [JsonProperty(PropertyName = "J")]
        public Journal Journal { get; set; }

        [JsonProperty(PropertyName = "C")]
        public Conference Conference { get; set; }

        [JsonProperty(PropertyName = "AA")]
        public List<Author> Authors { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MagEntityR
    {
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "F")]
        public List<Field> Fields { get; set; }

        [JsonProperty(PropertyName = "J")]
        public Journal Journal { get; set; }

        [JsonProperty(PropertyName = "C")]
        public Conference Conference { get; set; }

        [JsonProperty(PropertyName = "AA")]
        public List<Author> Authors { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MagEntityRId
    {
        [JsonProperty(PropertyName = "RId")]
        public List<long> ReferenceIds { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MagEntityAuId
    {
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "AA")]
        public List<Author> Authors { get; set; }
    }
}