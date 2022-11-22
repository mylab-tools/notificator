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
        /// Override to send notification to contacts
        /// </summary>
        Task SendNotificationToContactsAsync(string[] contacts, NotificationDto notification);

        /// <summary>
        /// Override to send notification to topic
        /// </summary>
        Task SendNotificationToTopicAsync(string topicId, NotificationDto notification);

        /// <summary>
        /// Override to implement logic for topic to contacts binding
        /// </summary>
        /// <param name="contacts">contacts</param>
        /// <param name="topicId">topic identifier</param>
        Task BindSubjectToTopicAsync(string[] contacts, string topicId);

        /// <summary>
        /// Override to implement logic for topic from contacts unbinding
        /// </summary>
        /// <param name="contacts">contacts</param>
        /// <param name="topicId">topic identifier</param>
        Task UnbindSubjectFromTopicAsync(string[] contacts, string topicId);
    }
}
