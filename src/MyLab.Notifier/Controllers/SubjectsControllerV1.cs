using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using MyLab.Db;
using MyLab.Notifier.Dal;

namespace MyLab.Notifier.Controllers
{
    [Route("v1/subjects/{subject_id}")]
    [ApiController]
    public class SubjectsControllerV1 : ControllerBase
    {
        private readonly IDbManager _db;

        public SubjectsControllerV1(IDbManager db)
        {
            _db = db;
        }

        [HttpPut("topics/{topic_id}")]
        public async Task<IActionResult> BindTopic([FromRoute(Name = "subject_id")] string subjectId, [FromRoute(Name = "topic_id")] string topicId)
        {
            if (string.IsNullOrWhiteSpace(topicId))
                return BadRequest("'topic_id' is not defined");
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' is not defined");

            await _db.DoOnce().Tab<TopicBindingDb>()
                .InsertOrUpdateAsync(
                    () => new TopicBindingDb
                    {
                        SubjectId = subjectId,
                        TopicId = topicId
                    },
                    b => b);

            return Ok();
        }

        [HttpDelete("topics/{topic_id}")]
        public async Task<IActionResult> UnbindTopic([FromRoute(Name = "subject_id")] string subjectId, [FromRoute(Name = "topic_id")] string topicId)
        {
            if (string.IsNullOrWhiteSpace(topicId))
                return BadRequest("'topic_id' is not defined");
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' is not defined");

            await _db.DoOnce().Tab<TopicBindingDb>()
                .Where(b => b.SubjectId == subjectId && b.TopicId == topicId)
                .DeleteAsync();

            return Ok();
        }

        [HttpGet("topics")]
        public async Task<IActionResult> GetSubjectTopics([FromRoute(Name = "subject_id")] string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' is not defined");

            var topics = await _db.DoOnce().Tab<TopicBindingDb>()
                .Where(b => b.SubjectId == subjectId)
                .Select(b => b.TopicId)
                .ToArrayAsync();

            return Ok(topics);
        }
    }
}
