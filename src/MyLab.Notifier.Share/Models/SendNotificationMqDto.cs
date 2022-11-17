using Newtonsoft.Json;

namespace MyLab.Notifier.Share.Models
{
    /// <summary>
    /// Send notification command DTO
    /// </summary>
    public class SendNotificationMqDto
    {
        /// <summary>
        /// Target contacts
        /// </summary>
        [JsonProperty("contacts")]
        public string[] Contacts { get; set; }
        /// <summary>
        /// Target topic
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }
        /// <summary>
        /// Notification to send
        /// </summary>
        [JsonProperty("notification")]
        public NotificationDto Notification { get; set; }
    }
}
