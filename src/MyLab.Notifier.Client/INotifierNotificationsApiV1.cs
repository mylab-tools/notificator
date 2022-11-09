using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.Notifier.Client.Models;

namespace MyLab.Notifier.Client
{
    /// <summary>
    /// Notifications API contract for v1
    /// </summary>
    [Api("v1/notifications")]
    public interface INotifierNotificationsApiV1
    {
        /// <summary>
        /// Sends notification to specific subject
        /// </summary>
        /// <param name="subjectId">subject unique identifier</param>
        /// <param name="notification">notification content</param>
        [Post("by-subject/{subject_id}")]
        Task SentNotificationToSubject([Path("subject_id")] string subjectId, [JsonContent] NotificationDto notification);

        /// <summary>
        /// Sends notification to specific topic
        /// </summary>
        /// <param name="topicId">topic unique identifier</param>
        /// <param name="notification">notification content</param>
        [Post("by-subject/{subject_id}")]
        Task SentNotificationToTopic([Path("subject_id")] string topicId, [JsonContent] NotificationDto notification);
    }
}
