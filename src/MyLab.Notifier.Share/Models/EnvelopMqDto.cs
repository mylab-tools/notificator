namespace MyLab.Notifier.Share.Models
{
    /// <summary>
    /// Describes envelop to interaction with mq consumer
    /// </summary>
    public class EnvelopMqDto
    {
        /// <summary>
        /// Send notification command
        /// </summary>
        public SendNotificationMqDto SendNotificationCmd { get; set; }

        /// <summary>
        /// Bind subject to topic command
        /// </summary>
        public BindingSubjectWithTopicMqDto BindSubjectToTopicCmd { get; set; }

        /// <summary>
        /// Unbind subject to topic command
        /// </summary>
        public BindingSubjectWithTopicMqDto UnbindSubjectToTopicCmd { get; set; }
    }
}
