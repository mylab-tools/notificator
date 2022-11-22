using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using MyLab.Notifier;
using MyLab.Notifier.Api;
using MyLab.Notifier.Api.Options;
using MyLab.Notifier.Client;
using MyLab.Notifier.Share.Dal;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Consuming;
using RabbitMQ.Client.Events;
using Xunit;
using Xunit.Abstractions;
using NotificationDto = MyLab.Notifier.Client.Models.NotificationDto;

namespace ApiFuncTests
{
    public partial class NotificationsApiV1Behavior :
        IClassFixture<TestApi<Program, INotifierNotificationsApiV1>>,
        IClassFixture<TmpDbFixture<NotificationsApiV1Behavior.InitialDbInitializer>>
    {
        [Fact]
        public async Task ShouldSendMqMessageForAllSubjectContacts()
        {
            //Arrange
            var initialContacts = new []
            {
                new ContactDb
                {
                    ChannelId = "foo-channel", 
                    SubjectId = "subject",
                    Value = "contact-1"
                },
                new ContactDb
                {
                    ChannelId = "foo-channel",
                    SubjectId = "subject",
                    Value = "contact-2"
                }
            };

            var testDb = await _db.CreateDbAsync(new AddContactsDbIniter(initialContacts));

            var testMqConsumer = new TestMqConsumer();

            var api = _api.StartWithProxy(s => 
                s.AddSingleton(testDb)
                    .AddRabbitConsumer("foo-channel", testMqConsumer)
                    .Configure<NotifierOptions>(opt =>
                    {
                        opt.Channels = new[]
                        {
                            new ChannelOptions { Id = "foo-channel" }
                        };
                    })
                    .AddRabbitEmulation()
                );

            var notification = GenerateNotification();

            //Act
            await api.SentNotificationToSubjectAsync("subject", notification);

            var mqMsg = testMqConsumer.LastMessage?.SendNotificationCmd;

            //Assert
            Assert.NotNull(mqMsg);
            Assert.Null(mqMsg.Topic);
            Assert.NotNull(mqMsg.Contacts);
            Assert.Equal("contact-1", mqMsg.Contacts[0]);
            Assert.Equal("contact-2", mqMsg.Contacts[1]);
            Assert.NotNull(mqMsg.Notification);
            Assert.Equal(notification.Title, mqMsg.Notification.Title);
            Assert.Equal(notification.Body, mqMsg.Notification.Body);
        }

        [Fact]
        public async Task ShouldSendSubjectContacts()
        {
            //Arrange
            var initialContacts = new[]
            {
                new ContactDb
                {
                    ChannelId = "foo-channel",
                    SubjectId = "subject",
                    Value = "contact-1"
                },
                new ContactDb
                {
                    ChannelId = "bar-channel",
                    SubjectId = "subject",
                    Value = "contact-2"
                }
            };

            var testDb = await _db.CreateDbAsync(new AddContactsDbIniter(initialContacts));

            var channel1TestMqConsumer = new TestMqConsumer();
            var channel2TestMqConsumer = new TestMqConsumer();

            var api = _api.StartWithProxy(s =>
                s.AddSingleton(testDb)
                    .AddRabbitConsumer("foo-channel", channel1TestMqConsumer)
                    .AddRabbitConsumer("bar-channel", channel2TestMqConsumer)
                    .Configure<NotifierOptions>(opt =>
                    {
                        opt.Channels = new[]
                        {
                            new ChannelOptions { Id = "foo-channel" },
                            new ChannelOptions { Id = "bar-channel" }
                        };
                    })
                    .AddRabbitEmulation()
            );

            var notification = GenerateNotification();

            //Act
            await api.SentNotificationToSubjectAsync("subject", notification);

            var ch1Msg = channel1TestMqConsumer.LastMessage?.SendNotificationCmd;
            var ch2Msg = channel2TestMqConsumer.LastMessage?.SendNotificationCmd;

            //Assert
            Assert.NotNull(ch1Msg);
            Assert.Null(ch1Msg.Topic);
            Assert.NotNull(ch1Msg.Contacts);
            Assert.Single(ch1Msg.Contacts);
            Assert.Equal("contact-1", ch1Msg.Contacts[0]);
            Assert.Equal(notification.Title, ch1Msg.Notification.Title);
            Assert.Equal(notification.Body, ch1Msg.Notification.Body);

            Assert.NotNull(ch2Msg);
            Assert.Null(ch2Msg.Topic);
            Assert.NotNull(ch2Msg.Contacts);
            Assert.Single(ch2Msg.Contacts);
            Assert.Equal("contact-2", ch2Msg.Contacts[0]);
            Assert.Equal(notification.Title, ch2Msg.Notification.Title);
            Assert.Equal(notification.Body, ch2Msg.Notification.Body);
        }

        [Fact]
        public async Task ShouldSendMqMessageForTopic()
        {
            //Arrange
            var testMqConsumer = new TestMqConsumer();

            var api = _api.StartWithProxy(s =>
                    s.AddRabbitConsumer("foo-channel", testMqConsumer)
                        .Configure<NotifierOptions>(opt =>
                            {
                                opt.Channels = new[]
                                {
                                    new ChannelOptions { Id = "foo-channel" }
                                };
                            })
                        .AddRabbitEmulation()
                    );

            var notification = GenerateNotification();

            //Act
            await api.SentNotificationToTopicAsync("topic", notification);

            var mqMsg = testMqConsumer.LastMessage?.SendNotificationCmd;

            //Assert
            Assert.NotNull(mqMsg);
            Assert.Equal("topic", mqMsg.Topic);
            Assert.Null(mqMsg.Contacts);
            Assert.NotNull(mqMsg.Notification);
            Assert.Equal(notification.Title, mqMsg.Notification.Title);
            Assert.Equal(notification.Body, mqMsg.Notification.Body);
        }

        [Fact]
        public async Task ShouldSendTopicMqMsgFroAllChannels()
        {
            //Arrange
            var channel1MqConsumer = new TestMqConsumer();
            var channel2MqConsumer = new TestMqConsumer();

            var api = _api.StartWithProxy(s =>
                s.AddRabbitConsumer("foo-channel", channel1MqConsumer)
                 .AddRabbitConsumer("bar-channel", channel2MqConsumer)
                 .Configure<NotifierOptions>(opt =>
                 {
                     opt.Channels = new[]
                     {
                         new ChannelOptions { Id = "foo-channel" },
                         new ChannelOptions { Id = "bar-channel" }
                     };
                 })
                 .AddRabbitEmulation()
            );
            
            var notification = GenerateNotification();

            var topicId = "topic";

            //Act
            await api.SentNotificationToTopicAsync(topicId, notification);

            var fooChMsg = channel1MqConsumer.LastMessage?.SendNotificationCmd;
            var barChMsg = channel2MqConsumer.LastMessage?.SendNotificationCmd;

            //Assert
            Assert.NotNull(fooChMsg);
            Assert.Equal("topic", fooChMsg.Topic);
            Assert.Null(fooChMsg.Contacts);
            Assert.Equal(notification.Title, fooChMsg.Notification.Title);
            Assert.Equal(notification.Body, fooChMsg.Notification.Body);

            Assert.NotNull(barChMsg);
            Assert.Equal("topic", barChMsg.Topic);
            Assert.Null(barChMsg.Contacts);
            Assert.Equal(notification.Title, barChMsg.Notification.Title);
            Assert.Equal(notification.Body, barChMsg.Notification.Body);
        }
    }
}
