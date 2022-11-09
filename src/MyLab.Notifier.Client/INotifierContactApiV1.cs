using System.Threading.Tasks;
using MyLab.ApiClient;
using MyLab.Notifier.Client.Models;

namespace MyLab.Notifier.Client
{
    /// <summary>
    /// Contact API contract for v1
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
        Task<int> AddContactAsync([Path("subject_id")] string subjectId, [JsonContent] ContactContent contact);

        /// <summary>
        /// Gets subject contacts
        /// </summary>
        /// <param name="subjectId">subject identifier</param>
        /// <returns>subject contacts</returns>
        [Get("by-subject/{subject_id}")]
        Task<Contact[]> GetContactsAsync([Path("subject_id")] string subjectId);

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
        /// <returns>Contact</returns>
        [Get("{contact_id}")]
        Task<Contact> GetContactAsync([Path("contact_id")] int contactId);
    }
}