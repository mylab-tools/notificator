using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLab.Notifier.Models;
using MyLab.Notifier.Services;

namespace MyLab.Notifier.Controllers
{
    [Route("v1/notifications")]
    [ApiController]
    public class NotificationsControllerV1 : ControllerBase
    {
        private readonly ISenderService _senderService;

        public NotificationsControllerV1(ISenderService senderService)
        {
            _senderService = senderService;
        }

        [HttpPost("by-subject/{subject_id}")]
        public async Task<IActionResult> SendNotificationToSubject([FromRoute(Name = "subject_id")] string subjectId, [FromBody] NotificationDto notification)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
                return BadRequest("'subject_id' is not defined");

            if (!ValidateNotification(notification, out var validationError))
                return BadRequest(validationError);

            await _senderService.SendNotificationToSubjectAsync(subjectId, notification);

            return Ok();
        }

        [HttpPost("by-topic/{topic_id}")]
        public async Task<IActionResult> SendNotificationToTopic([FromRoute(Name = "topic_id")] string topicId, [FromBody] NotificationDto notification)
        {
            if (string.IsNullOrWhiteSpace(topicId))
                return BadRequest("'topic_id' is not defined");

            if (!ValidateNotification(notification, out var validationError))
                return BadRequest(validationError);

            await _senderService.SendNotificationToTopicAsync(topicId, notification);

            return Ok();
        }

        [HttpPost("by-subject")]
        public IActionResult SendNotificationToSubject()
        {
            return BadRequest("'subject_id' is not defined");
        }

        [HttpPost("by-topic")]
        public IActionResult SendNotificationToTopic()
        {
            return BadRequest("'topic_id' is not defined");
        }

        bool ValidateNotification(NotificationDto notification, out string errorDescription)
        {
            if (notification == null)
            {
                errorDescription = "Notification body is not defined";
                return false;
            }

            throw new NotImplementedException();

            return true;
        }
    }
}
