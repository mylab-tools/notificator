using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#if CLIENTLIB
namespace MyLab.Notifier.Client.Models
#endif
#if SHARELIB
namespace MyLab.Notifier.Share.Models
#endif
{
    /// <summary>
    /// Describes a notification
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Contains notification content
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
        /// <summary>
        /// Link to notification target object
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }
        /// <summary>
        /// Level <see cref="NotificationLevel"/>
        /// </summary>
        [JsonProperty("level")]
        public NotificationLevel Level { get; set; }
    }

    /// <summary>
    /// Enums notification levels
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationLevel
    {
        /// <summary>
        /// Default level - neutral
        /// </summary>
        Info,
        /// <summary>
        /// Special notification
        /// </summary>
        Warning,
        /// <summary>
        /// Very special notification
        /// </summary>
        Danger
    }
}
