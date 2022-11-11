using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.Notifier.Client.Models;

namespace MyLab.Notifier.Client
{
    /// <summary>
    /// ContactDto API contract for v1
    /// </summary>
    [Api("v1/contacts")]
    public interface INotifierContactApiV1
    {
        /// <summary>
        /// Adds new contact for subject
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <param name="contact">new contact</param>
        /// <returns>new contact identifier</returns>
        [Post("by-subject/{subject_id}")]
        Task<int> AddContactAsync([Path("subject_id")] string subjectId, [JsonContent] ContactContentDto contact);

        /// <summary>
        /// Gets subject contacts
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <returns>subject contacts</returns>
        [Get("by-subject/{subject_id}")]
        Task<ContactDto[]> GetContactsAsync([Path("subject_id")] string subjectId);

        /// <summary>
        /// Deletes contact by identifier
        /// </summary>
        /// <param name="contactId">contact identifier</param>
        [Delete("{contact_id}")]
        Task DeleteContactAsync([Path("contact_id")] int contactId);

        /// <summary>
        /// Gets contact by identifier
        /// </summary>
        /// <param name="contactId">contact identifier</param>
        /// <returns>ContactDto</returns>
        [Get("{contact_id}")]
        Task<ContactDto> GetContactAsync([Path("contact_id")] int contactId);
    }
}