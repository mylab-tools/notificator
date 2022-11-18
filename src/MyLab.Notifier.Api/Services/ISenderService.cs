using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Db;
using MyLab.Log.Dsl;
using MyLab.Notifier.Dal;
using MyLab.Notifier.Options;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient.Publishing;

namespace MyLab.Notifier.Services
{
    public interface ISenderService
    {
        Task SendNotificationToSubjectAsync(string subjectId, NotificationDto notification);
        Task SendNotificationToTopicAsync(string subjectId, NotificationDto notification);
    }

    class SenderService : ISenderService
    {
        private readonly IRabbitPublisher _publisher;
        private readonly IDbManager _dbManager;
        private readonly NotifierOptions _options;
        private readonly IDslLogger _logger;

        public SenderService(
            IRabbitPublisher publisher, 
            IDbManager dbManager, 
            IOptions<NotifierOptions> options,
            ILogger<SenderService> log)
            :this(publisher, dbManager, options.Value, log)
        {
            
        }

        public SenderService(
            IRabbitPublisher publisher, 
            IDbManager dbManager, 
            NotifierOptions options,
            ILogger<SenderService> log)
        {
            _publisher = publisher;
            _dbManager = dbManager;
            _options = options;
            _logger = log.Dsl();
        }

        public async Task SendNotificationToSubjectAsync(string subjectId, NotificationDto notification)
        {
            var contacts = await _dbManager.DoOnce()
                .Tab<ContactDb>()
                .Where(c => c.SubjectId == subjectId)
                .GroupBy(c => c.ChannelId, c => c.Value)
                .ToDictionaryAsync(g => g.Key, g => g);

            foreach (var contact in contacts)
            {
                var channelId = contact.Key;

                if (string.IsNullOrWhiteSpace(channelId))
                    throw new InvalidOperationException("Channel id is not defined");

                var mqNotifDto = new SendNotificationMqDto
                {
                    Contacts = contact.Value.ToArray(),
                    Notification = notification
                };

                var mqEnvelop = new EnvelopMqDto
                {
                    SendNotificationCmd = mqNotifDto
                };

                try
                {
                    _publisher
                        .IntoDefault(channelId)
                        .SetJsonContent(mqEnvelop)
                        .Publish();
                }
                catch (Exception e)
                {
                    _logger.Error("Send notification error", e)
                        .AndFactIs("channel-id", channelId)
                        .Write();
                }
            }
        }

        public Task SendNotificationToTopicAsync(string topicId, NotificationDto notification)
        {
            if(_options.Channels == null) return Task.CompletedTask;

            foreach (var optionsChannel in _options.Channels)
            {
                var channelId = optionsChannel.Id;

                if (string.IsNullOrWhiteSpace(channelId))
                    throw new InvalidOperationException("Channel id is not defined");

                var mqNotifDto = new SendNotificationMqDto
                {
                    Topic = topicId,
                    Notification = notification
                };

                var mqEnvelop = new EnvelopMqDto
                {
                    SendNotificationCmd = mqNotifDto
                };

                try
                {
                    _publisher
                        .IntoDefault(channelId)
                        .SetJsonContent(mqEnvelop)
                        .Publish();
                }
                catch (Exception e)
                {
                    _logger.Error("Send notification error", e)
                        .AndFactIs("channel-id", channelId)
                        .Write();
                }
            }

            return Task.CompletedTask;
        }
    }
}
