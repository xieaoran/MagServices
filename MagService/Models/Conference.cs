#region

using Newtonsoft.Json;

#endregion

namespace MagService.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Conference
    {
        [JsonProperty(PropertyName = "CId")]
        public long Id { get; set; }
    }
}