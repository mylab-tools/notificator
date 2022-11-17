using Newtonsoft.Json;

#if CLIENTLIB
namespace MyLab.Notifier.Client.Models
#endif
#if SHARELIB
namespace MyLab.Notifier.Share.Models
#endif
{
    public class ContactDto : ContactContentDto
    {
        [JsonProperty("id")] public int? Id { get; set; }
    }
}