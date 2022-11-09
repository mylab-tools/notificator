using System.Collections.Generic;
using Newtonsoft.Json;

#if SERVER_APP
namespace MyLab.Notifier.Models
#else
namespace MyLab.Notifier.Client.Models
#endif
{
    public class ContactContent
    {
        [JsonProperty("value")]
        public string? Value { get; set; }
        [JsonProperty("channelId")]
        public string? ChannelId { get; set; }
        [JsonProperty("labels")]
        public Dictionary<string, string>? Labels { get; set; }
    }
}
