#region

using Newtonsoft.Json;

#endregion

namespace MagService.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Author
    {
        [JsonProperty(PropertyName = "AuId")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "AfId")]
        public long AffiliationId { get; set; }
    }
}