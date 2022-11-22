
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.Share;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Publishing;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests.ChannelAdapter
{
    public class ChannelAdapterBehavior
    {
        private readonly ITestOutputHelper _output;

        public ChannelAdapterBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldSendNotificationToSubject()
        {
            //Arrange
            var channelMock = new Mock<INotifierChannel>();

            string[] receivedContacts = null;
            NotificationDto receivedNotification = null;

            channelMock.Setup(ch => ch
                .SendNotificationToContactsAsync(It.IsAny<string[]>(), It.IsAny<NotificationDto>()))
                .Callback<string[], NotificationDto>((contacts, notification) =>
                    {
                        receivedContacts = contacts;
                        receivedNotification = notification;
                    }
            );

            var consumer = new NotifierEnvelopConsumer(channelMock.Object);

            var config = (IConfiguration)new ConfigurationBuilder()
                .Build();

            var sp = new ServiceCollection()
                .AddLogging(l => l.AddFilter(l => true).AddXUnit(_output))
                .AddNotifierChannelLogic(consumer, config)
                .AddRabbitEmulation()
                .Configure<NotifierChannelAdapterOptions>(opt =>
                {
                    opt.MqQueue = "foo";
                })
                .ConfigureRabbit(opts =>
                {
                    opts.DefaultPub = new PublishOptions
                    {
                        RoutingKey = "foo"
                    };
                })
                .BuildServiceProvider();

            var mqPublisher = sp.GetService<IRabbitPublisher>();

            var envelop = new EnvelopMqDto
            {
                SendNotificationCmd = new SendNotificationMqDto
                {
                    Contacts = new []{ "bar" },
                    Notification = new NotificationDto{ Title = "baz" }
                }
            };

            //Act
            mqPublisher?
                .IntoDefault()
                .SetJsonContent(envelop)
                .Publish();

            //Assert
            Assert.NotNull(receivedContacts);
            Assert.Single(receivedContacts);
            Assert.Equal("bar", receivedContacts[0]);

            Assert.NotNull(receivedNotification);
            Assert.Equal("baz", receivedNotification.Title);
        }

        [Fact]
        public void ShouldSendNotificationToTopic()
        {
            //Arrange
            var channelMock = new Mock<INotifierChannel>();

            string receivedTopic = null;
            NotificationDto receivedNotification = null;

            channelMock.Setup(ch => ch
                .SendNotificationToTopicAsync(It.IsAny<string>(), It.IsAny<NotificationDto>()))
                .Callback<string, NotificationDto>((topic, notification) =>
                {
                    receivedTopic = topic;
                    receivedNotification = notification;
                }
            );

            var consumer = new NotifierEnvelopConsumer(channelMock.Object);

            var config = (IConfiguration)new ConfigurationBuilder()
                .Build();

            var sp = new ServiceCollection()
                .AddLogging(l => l.AddFilter(l => true).AddXUnit(_output))
                .AddNotifierChannelLogic(consumer, config)
                .AddRabbitEmulation()
                .Configure<NotifierChannelAdapterOptions>(opt =>
                {
                    opt.MqQueue = "foo";
                })
                .ConfigureRabbit(opts =>
                {
                    opts.DefaultPub = new PublishOptions
                    {
                        RoutingKey = "foo"
                    };
                })
                .BuildServiceProvider();

            var mqPublisher = sp.GetService<IRabbitPublisher>();

            var envelop = new EnvelopMqDto
            {
                SendNotificationCmd = new SendNotificationMqDto
                {
                    Topic = "bar",
                    Notification = new NotificationDto { Title = "baz" }
                }
            };

            //Act
            mqPublisher?
                .IntoDefault()
                .SetJsonContent(envelop)
                .Publish();

            //Assert
            Assert.NotNull(receivedTopic);
            Assert.Equal("bar", receivedTopic);

            Assert.NotNull(receivedNotification);
            Assert.Equal("baz", receivedNotification.Title);
        }

        [Fact]
        public void ShouldBindSubjectToTopic()
        {
            //Arrange
            var channelMock = new Mock<INotifierChannel>();

            string[] receivedContacts = null;
            string receivedTopicId = null;

            channelMock.Setup(ch => ch
                .BindSubjectToTopicAsync(It.IsAny<string[]>(), It.IsAny<string>()))
                .Callback<string[], string>((contacts, topicId) =>
                    {
                        receivedContacts = contacts;
                        receivedTopicId = topicId;
                    }
            );

            var consumer = new NotifierEnvelopConsumer(channelMock.Object);

            var config = (IConfiguration)new ConfigurationBuilder()
                .Build();

            var sp = new ServiceCollection()
                .AddLogging(l => l.AddFilter(l => true).AddXUnit(_output))
                .AddNotifierChannelLogic(consumer, config)
                .AddRabbitEmulation()
                .Configure<NotifierChannelAdapterOptions>(opt =>
                {
                    opt.MqQueue = "foo";
                })
                .ConfigureRabbit(opts =>
                {
                    opts.DefaultPub = new PublishOptions
                    {
                        RoutingKey = "foo"
                    };
                })
                .BuildServiceProvider();

            var mqPublisher = sp.GetService<IRabbitPublisher>();

            var envelop = new EnvelopMqDto
            {
                BindSubjectToTopicCmd = new BindingSubjectWithTopicMqDto
                {
                    Contacts = new []{ "bar" },
                    TopicId = "baz"
                }
            };

            //Act
            mqPublisher?
                .IntoDefault()
                .SetJsonContent(envelop)
                .Publish();

            //Assert
            Assert.NotNull(receivedContacts);
            Assert.Single(receivedContacts);
            Assert.Equal("bar", receivedContacts[0]);

            Assert.NotNull(receivedTopicId);
            Assert.Equal("baz", receivedTopicId);
        }

        [Fact]
        public void ShouldUnbindSubjectFromTopic()
        {
            //Arrange
            var channelMock = new Mock<INotifierChannel>();

            string[] receivedContacts = null;
            string receivedTopicId = null;

            channelMock.Setup(ch => ch
                .UnbindSubjectFromTopicAsync(It.IsAny<string[]>(), It.IsAny<string>()))
                .Callback<string[], string>((contacts, topicId) =>
                {
                    receivedContacts = contacts;
                    receivedTopicId = topicId;
                }
            );

            var consumer = new NotifierEnvelopConsumer(channelMock.Object);

            var config = (IConfiguration)new ConfigurationBuilder()
                .Build();

            var sp = new ServiceCollection()
                .AddLogging(l => l.AddFilter(l => true).AddXUnit(_output))
                .AddNotifierChannelLogic(consumer, config)
                .AddRabbitEmulation()
                .Configure<NotifierChannelAdapterOptions>(opt =>
                {
                    opt.MqQueue = "foo";
                })
                .ConfigureRabbit(opts =>
                {
                    opts.DefaultPub = new PublishOptions
                    {
                        RoutingKey = "foo"
                    };
                })
                .BuildServiceProvider();

            var mqPublisher = sp.GetService<IRabbitPublisher>();

            var envelop = new EnvelopMqDto
            {
                UnbindSubjectFromTopicCmd = new BindingSubjectWithTopicMqDto
                {
                    Contacts = new[] { "bar" },
                    TopicId = "baz"
                }
            };

            //Act
            mqPublisher?
                .IntoDefault()
                .SetJsonContent(envelop)
                .Publish();

            //Assert
            Assert.NotNull(receivedContacts);
            Assert.Single(receivedContacts);
            Assert.Equal("bar", receivedContacts[0]);

            Assert.NotNull(receivedTopicId);
            Assert.Equal("baz", receivedTopicId);
        }
    }
}
