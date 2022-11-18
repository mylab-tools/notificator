using Newtonsoft.Json;

namespace MyLab.Notifier.Share.Models
{
    /// <summary>
    /// Binding command with contacts and topic 
    /// </summary>
    public class BindingSubjectWithTopicMqDto
    {
        /// <summary>
        /// Contacts
        /// </summary>
        [JsonProperty("contacts")]
        public string[] Contacts { get; set; }
        /// <summary>
        /// Channel specific topic identifier
        /// </summary>
        [JsonProperty("topicId")]
        public string TopicId { get; set; }
    }
}