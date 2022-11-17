using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

#if CLIENTLIB
namespace MyLab.Notifier.Client.Models
#endif
#if SHARELIB
namespace MyLab.Notifier.Share.Models
#endif
{
    public class ContactContentDto
    {
        [JsonProperty("value")]
        public string? Value { get; set; }
        [JsonProperty("channelId")]
        public string? ChannelId { get; set; }
        [JsonProperty("labels")]
        public Dictionary<string, string>? Labels { get; set; }
    }
}
