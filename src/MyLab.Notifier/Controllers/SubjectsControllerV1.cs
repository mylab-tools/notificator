using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyLab.Notifier.Controllers
{
    [Route("v1/subjects/{subject_id}")]
    [ApiController]
    public class SubjectsControllerV1 : ControllerBase
    {
        [HttpPost("topics")]
        public async Task<IAsyncResult> BindSubjectToTopic([FromRoute(Name = "subject_id")] string subjectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("topics")]
        public async Task<IAsyncResult> GetSubjectTopics([FromRoute(Name = "subject_id")] string subjectId)
        {
            throw new NotImplementedException();
        }
    }
}
