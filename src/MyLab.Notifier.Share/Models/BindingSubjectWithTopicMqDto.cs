using Newtonsoft.Json;

namespace MyLab.Notifier.Share.Models
{
    /// <summary>
    /// Binding subject with topic command
    /// </summary>
    public class BindingSubjectWithTopicMqDto
    {
        /// <summary>
        /// Subject identifier
        /// </summary>
        [JsonProperty("subjectId")]
        public string SubjectId { get; set; }
        /// <summary>
        /// Channel specific topic identifier
        /// </summary>
        [JsonProperty("topicId")]
        public string TopicId { get; set; }
    }
}