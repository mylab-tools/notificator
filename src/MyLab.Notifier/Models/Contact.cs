using Newtonsoft.Json;

#if SERVER_APP
namespace MyLab.Notifier.Models
#else
namespace MyLab.Notifier.Client.Models
#endif
{
    public class Contact : ContactContent
    {
        [JsonProperty("id")] public int? Id { get; set; }
    }
}