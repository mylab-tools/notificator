using Newtonsoft.Json;

namespace MyLab.Notifier.Models
{
    public class SendNotificationMqDto
    {
        [JsonProperty("contacts")]
        public string[] Contacts { get; set; }
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("notification")]
        public NotificationDto Notification { get; set; }
    }
}
