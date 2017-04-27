#region

using Newtonsoft.Json;

#endregion

namespace MagService.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Journal
    {
        [JsonProperty(PropertyName = "JId")]
        public long Id { get; set; }
    }
}