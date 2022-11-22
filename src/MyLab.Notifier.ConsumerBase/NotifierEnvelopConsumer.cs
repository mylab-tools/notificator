using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.Log.Dsl;
using MyLab.Notifier.Share;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient.Consuming;

namespace MyLab.Notifier.ChannelAdapter
{
    public class NotifierEnvelopConsumer : RabbitConsumer<EnvelopMqDto>
    {
        private readonly INotifierChannel _channelLogic;
        private readonly IDslLogger _log;

        public NotifierEnvelopConsumer(INotifierChannel channelLogic, ILogger<NotifierEnvelopConsumer> logger = null)
        {
            _channelLogic = channelLogic;
            _log = logger?.Dsl();
        }

        protected override async Task ConsumeMessageAsync(ConsumedMessage<EnvelopMqDto> consumedMessage)
        {
            if (consumedMessage.Content == null)
            {
                _log?.Warning("Input envelop is null").Write();
                return;
            }

            var envlp = consumedMessage.Content;

            if (envlp.SendNotificationCmd != null)
            {
                if (envlp.SendNotificationCmd.Topic != null)
                {
                    try
                    {
                        await _channelLogic.SendNotificationToTopicAsync(envlp.SendNotificationCmd.Topic, envlp.SendNotificationCmd.Notification);
                    }
                    catch (Exception e)
                    {
                        _log?.Error("Send notification to topic error", e).Write();
                    }
                }

                if (envlp.SendNotificationCmd.Contacts != null && envlp.SendNotificationCmd.Contacts.Length > 0)
                {
                    try
                    {
                        await _channelLogic.SendNotificationToContactsAsync(envlp.SendNotificationCmd.Contacts, envlp.SendNotificationCmd.Notification);
                    }
                    catch (Exception e)
                    {
                        _log?.Error("Send notification to contacts error", e).Write();
                    }
                }
            }

            if (envlp.BindSubjectToTopicCmd != null)
            {
                try
                {
                    await _channelLogic.BindSubjectToTopicAsync(envlp.BindSubjectToTopicCmd.Contacts, envlp.BindSubjectToTopicCmd.TopicId);
                }
                catch (Exception e)
                {
                    _log?.Error("Bind subject to topic error", e).Write();
                }
            }

            if (envlp.UnbindSubjectFromTopicCmd != null)
            {
                try
                {
                    await _channelLogic.UnbindSubjectFromTopicAsync(envlp.UnbindSubjectFromTopicCmd.Contacts, envlp.UnbindSubjectFromTopicCmd.TopicId);
                }
                catch (Exception e)
                {
                    _log?.Error("Unbind subject from topic error", e).Write();
                }
            }
        }
    }
}