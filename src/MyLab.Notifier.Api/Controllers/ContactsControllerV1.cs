using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using MyLab.Db;
using MyLab.Notifier.Dal;
using MyLab.Notifier.Share.Models;

namespace MyLab.Notifier.Controllers
{
    [Route("v1/contacts")]
    [ApiController]
    public class ContactsControllerV1 : ControllerBase
    {
        private readonly IDbManager _db;

        public ContactsControllerV1(IDbManager db)
        {
            _db = db;
        }

        [HttpPost("by-subject/{subject_id}")]
        public async Task<IActionResult> AddContact([FromRoute(Name = "subject_id")] string subjectId, [FromBody] ContactContentDto contact)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' not defined");
            if(contact == null)
                return BadRequest("ContactDto data not defined");
            if (contact.ChannelId == null)
                return BadRequest("Channel id is not defined");
            if (contact.Value == null)
                return BadRequest("ContactDto value is not defined");

            await using var dataConn = _db.Use();

            int contactId = -1;

            await dataConn.PerformAutoTransactionAsync(async connection =>
            {
                contactId = (int)(long)await connection.Tab<ContactDb>()
                    .InsertWithIdentityAsync(() => new ContactDb
                    {
                        ChannelId = contact.ChannelId,
                        SubjectId = subjectId,
                        Value = contact.Value,

                    });

                if (contact.Labels != null && contact.Labels.Count > 0)
                {
                    var labelsDb = contact.Labels.Select(lkv =>
                        new ContactLabelDb()
                        {
                            ContactId = contactId,
                            Name = lkv.Key,
                            Value = lkv.Value
                        });
                    await connection.Tab<ContactLabelDb>().BulkCopyAsync(labelsDb);
                }
            });

            return Ok(contactId);
        }

        [HttpPost("by-subject")]
        public IActionResult AddContact()
        {
            return BadRequest("'subject_id' not defined");
        }

        [HttpGet("by-subject/{subject_id}")]
        public async Task<IActionResult> GetContacts([FromRoute(Name = "subject_id")] string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' not defined");

            var contacts = await _db.DoOnce()
                .Tab<ContactDb>()
                .LoadWith(c => c.Labels)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    ChannelId = c.ChannelId,
                    Value = c.Value,
                    Labels = c.Labels.ToDictionary(l => l.Name, l=>l.Value)
                })
                .ToArrayAsync();

            return Ok(contacts);
        }

        [HttpGet("by-subject")]
        public IActionResult GetContacts()
        {
            return BadRequest("'subject_id' not defined");
        }

        [HttpDelete("{contact_id}")]
        public async Task<IActionResult> DeleteContact([FromRoute(Name = "contact_id")] int? contactId)
        {
            if (!contactId.HasValue)
                return BadRequest("'contact_id' not defined");

            await _db.DoOnce()
                .Tab<ContactDb>()
                .Where(c => c.Id == contactId.Value)
                .DeleteAsync();

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteContact()
        {
            return BadRequest("'contact_id' not defined");
        }

        [HttpGet("{contact_id}")]
        public async Task<IActionResult> GetContact([FromRoute(Name = "contact_id")] int? contactId)
        {
            if (!contactId.HasValue)
                return BadRequest("'contact_id' not defined");

            var found = await _db.DoOnce()
                .Tab<ContactDb>()
                .LoadWith(c => c.Labels)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    ChannelId = c.ChannelId,
                    Value = c.Value,
                    Labels = c.Labels.ToDictionary(l => l.Name, l => l.Value)
                })
                .FirstOrDefaultAsync(c => c.Id == contactId.Value);

            if (found == null)
                return NotFound();

            return Ok(found);
        }

        [HttpGet]
        public IActionResult GetContact()
        {
            return BadRequest("'contact_id' not defined");
        }
    }
}
