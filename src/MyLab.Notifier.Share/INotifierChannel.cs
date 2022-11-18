using System.Threading.Tasks;
using MyLab.Notifier.Share.Models;

namespace MyLab.Notifier.Share
{
    /// <summary>
    /// Implements external channel interaction
    /// </summary>
    public interface INotifierChannel
    {
        /// <summary>
        /// Sends notification to contacts
        /// </summary>
        Task SendNotificationToContactsAsync(string[] contacts, NotificationDto notification);

        /// <summary>
        /// Sends notification to subject
        /// </summary>
        Task SendNotificationToTopicAsync(string topicId, NotificationDto notification);

        /// <summary>
        /// Binds subject to topic
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <param name="topicId">topic identifier</param>
        Task BindSubjectToTopicAsync(string subjectId, string topicId);

        /// <summary>
        /// Unbinds subject from topic
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <param name="topicId">topic identifier</param>
        Task UnbindSubjectFromTopicAsync(string subjectId, string topicId);
    }
}
