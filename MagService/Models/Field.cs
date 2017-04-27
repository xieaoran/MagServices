#region

using Newtonsoft.Json;

#endregion

namespace MagService.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Field
    {
        [JsonProperty(PropertyName = "FId")]
        public long Id { get; set; }
    }
}