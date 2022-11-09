using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.Notifier.Client
{
    /// <summary>
    /// Subject API contract for v1
    /// </summary>
    [Api("v1/subjects")]
    public interface INotifierSubjectApiV1
    {
        /// <summary>
        /// Binds subject to topic
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <param name="topicId">topic identifier</param>
        [Put("{subject_id}/topics/{topic_id}")]
        Task BindTopicAsync([Path("subject_id")] string subjectId, [Path("topic_id")] string topicId);

        /// <summary>
        /// Unbinds subject with topic
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <param name="topicId">topic identifier</param>
        [Delete("{subject_id}/topics/{topic_id}")]
        Task UnbindTopicAsync([Path("subject_id")] string subjectId, [Path("topic_id")] string topicId);

        /// <summary>
        /// Gets subject topics
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <returns>topic list</returns>
        [Get("{subject_id}/topics")]
        Task<string[]> GetSubjectTopicsAsync([Path("subject_id")] string subjectId);
    }
}