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
        /// <param name="topic">topic</param>
        [Put("{subject_id}/topics")]
        Task BindSubjectToTopicAsync([Path("subject_id")] string subjectId, [StringContent] string topic);

        /// <summary>
        /// Gets subject topics
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <returns>topic list</returns>
        [Get("{subject_id}/topics")]
        Task<string[]> GetSubjectTopicsAsync([Path("subject_id")] string subjectId);
    }
}