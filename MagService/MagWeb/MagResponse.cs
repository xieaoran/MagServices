#region

using System.Collections.Generic;
using MagService.Models;
using Newtonsoft.Json;

#endregion

namespace MagService.MagWeb
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MagResponse
    {
        [JsonProperty(PropertyName = "entities")]
        public IEnumerable<MagEntity> MagEntities { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MagResponseLId
    {
        [JsonProperty(PropertyName = "entities")]
        public IEnumerable<MagEntityR> MagEntityLIds { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MagResponseRId
    {
        [JsonProperty(PropertyName = "entities")]
        public IEnumerable<MagEntityRId> MagEntityRIds { get; set; }
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class MagResponseAuId
    {
        [JsonProperty(PropertyName = "entities")]
        public IEnumerable<MagEntityAuId> MagEntityAuIds { get; set; }
    }
}