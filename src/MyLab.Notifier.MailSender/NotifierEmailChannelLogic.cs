using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLab.Db;
using MyLab.Log.Dsl;
using MyLab.Notifier.MailSender.Options;
using MyLab.Notifier.MailSender.Services;
using MyLab.Notifier.Share;
using MyLab.Notifier.Share.Dal;
using MyLab.Notifier.Share.Models;

namespace MyLab.Notifier.MailSender
{
    class NotifierEmailChannelLogic : INotifierChannel
    {
        private readonly IEmailSender _emailSender;
        private readonly MailChannelOptions _options;
        private readonly IDslLogger _log;
        private readonly IDbManager _db;

        public NotifierEmailChannelLogic(
            IEmailSender emailSender,
            IDbManager db,
            IOptions<MailChannelOptions> options, 
            ILogger<NotifierEmailChannelLogic> logger)
            :this(emailSender, db,options.Value, logger)
        {
            
        }

        public NotifierEmailChannelLogic(
            IEmailSender emailSender,
            IDbManager db,
            MailChannelOptions options, 
            ILogger<NotifierEmailChannelLogic> logger)
        {
            _emailSender = emailSender;
            _options = options;
            _log = logger.Dsl();
            _db = db;
        }

        public async Task SendNotificationToContactsAsync(string[] contacts, NotificationDto notification)
        {
            for (int i = 0; i < contacts.Length-1; i += _options.BccLimit)
            {
                var batch = contacts
                    .Skip(i)
                    .Take(_options.BccLimit)
                    .ToArray();

                await CoreSendNotificationAsync(batch, notification);
            }
        }

        public async Task SendNotificationToTopicAsync(string topicId, NotificationDto notification)
        { 
            var contacts = await _db.DoOnce().Tab<TopicBindingDb>()
                .Where(b => b.TopicId == topicId)
                .SelectMany(b => b.Contacts)
                .Where(c => c.ChannelId == "email")
                .Select(c => c.Value)
                .ToArrayAsync();

            await CoreSendNotificationAsync(contacts, notification);
        }

        public Task BindSubjectToTopicAsync(string[] contacts, string topicId)
        {
            return Task.CompletedTask;
        }

        public Task UnbindSubjectFromTopicAsync(string[] contacts, string topicId)
        {
            return Task.CompletedTask;
        }

        Task CoreSendNotificationAsync(string[] contacts, NotificationDto notification)
        {
            return _emailSender.SendNotificationAsync(contacts, new EmailEnvelop
            {
                Subject = notification.Title,
                Body = notification.Body
            });
        }
    }
}