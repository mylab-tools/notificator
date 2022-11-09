using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLab.Notifier.Models;

namespace MyLab.Notifier.Controllers
{
    [Route("v1/notifications")]
    [ApiController]
    public class NotificationsControllerV1 : ControllerBase
    {
        [HttpPost("by-subject/{subject_id}")]
        public Task<IActionResult> SentNotificationToSubject([FromRoute(Name = "subject_id")] string subjectId, [FromBody] NotificationDto notification)
        {
            throw new NotImplementedException();
        }

        [HttpPost("by-subject/{topic_id}")]
        public Task<IActionResult> SentNotificationToTopic([FromRoute(Name = "topic_id")] string topicId, [FromBody] NotificationDto notification)
        {
            throw new NotImplementedException();
        }
    }
}
