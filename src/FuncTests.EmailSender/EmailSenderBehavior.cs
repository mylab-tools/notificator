using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyLab.Db;
using MyLab.DbTest;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.EmailSender;
using MyLab.Notifier.EmailSender.Services;
using MyLab.Notifier.Share.Dal;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Publishing;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests.EmailSender
{
    public class EmailSenderBehavior : IClassFixture<TmpDbFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture _db;

        public EmailSenderBehavior(
            TmpDbFixture dbFixture,
            ITestOutputHelper output)
        {
            dbFixture.Output = output;

            _output = output;
            _db = dbFixture;
        }

        [Fact]
        public void ShouldSendToContacts()
        {
            //Arrange

            var db = new Mock<IDbManager>();

            var testMailSender = new TestEmailSender();
            
            var services = new ServiceCollection()
                .AddLogging(l => l
                    .ClearProviders()
                    .AddFilter(l => true)
                    .AddXUnit(_output)
                )
                .AddMailSenderLogic()
                .AddRabbitEmulation()
                .AddSingleton<IEmailSender>(testMailSender)
                .AddSingleton(db.Object)
                
                .ConfigureNotifierChannelLogic(opt => opt.MqQueue = "qox")
                .ConfigureRabbit(opt => opt.DefaultPub = new PublishOptions{RoutingKey = "qox"})

                .BuildServiceProvider();


            var publisher = services.GetService<IRabbitPublisher>();

            var mqEnvelop = new EnvelopMqDto
            {
                SendNotificationCmd = new SendNotificationMqDto
                {
                    Contacts = new []{ "foo", "bar" },
                    Notification = new NotificationDto
                    {
                        Title = "baz",
                        Body = "qux"
                    }
                }
            };

            //Act
            publisher?
                .IntoDefault()
                .SetJsonContent(mqEnvelop)
                .Publish();

            var actualContacts = testMailSender.Contacts;
            var actualMsg = testMailSender.Envelop;

            //Assert
            Assert.NotNull(actualContacts);
            Assert.Equal(2, actualContacts.Length);
            Assert.Equal("foo", actualContacts[0]);
            Assert.Equal("bar", actualContacts[1]);

            Assert.NotNull(actualMsg);
            Assert.Equal("baz", actualMsg.Subject);
            Assert.Equal("qux", actualMsg.Body);
        }

        [Fact]
        public async Task ShouldSendToTopic()
        {
            //Arrange
            var testMailSender = new TestEmailSender();

            var dbInitializer = new Mock<ITestDbInitializer>();
            dbInitializer.Setup(i => i.InitializeAsync(It.IsAny<DataConnection>()))
                .Callback<DataConnection>(dc =>
                {
                    dc.CreateTable<ContactDb>();
                    dc.CreateTable<TopicBindingDb>();

                    dc.Tab<ContactDb>().BulkCopyAsync(new []
                    {
                        new ContactDb{ ChannelId = "wrong", SubjectId = "qux", Value = "qux1@host.ru"},
                        new ContactDb{ ChannelId = "email", SubjectId = "qux", Value = "qux2@host.ru"},
                        new ContactDb{ ChannelId = "email", SubjectId = "qux", Value = "qux3@host.ru"}
                    });

                    dc.Tab<TopicBindingDb>().BulkCopyAsync(new[]
                    {
                        new TopicBindingDb{ SubjectId = "qux", TopicId = "foo"}
                    });
                });

            var db = await _db.CreateDbAsync(dbInitializer.Object);

            var services = new ServiceCollection()
                .AddLogging(l => l
                    .ClearProviders()
                    .AddFilter(l => true)
                    .AddXUnit(_output)
                )
                .AddMailSenderLogic()
                .AddRabbitEmulation()
                .AddSingleton<IEmailSender>(testMailSender)
                .AddSingleton(db)

                .ConfigureNotifierChannelLogic(opt => opt.MqQueue = "qox")
                .ConfigureRabbit(opt => opt.DefaultPub = new PublishOptions { RoutingKey = "qox" })

                .BuildServiceProvider();


            var publisher = services.GetService<IRabbitPublisher>();

            var mqEnvelop = new EnvelopMqDto
            {
                SendNotificationCmd = new SendNotificationMqDto
                {
                    Topic = "foo",
                    Notification = new NotificationDto
                    {
                        Title = "bar",
                        Body = "baz"
                    }
                }
            };

            //Act
            publisher?
                .IntoDefault()
                .SetJsonContent(mqEnvelop)
                .Publish();

            var actualContacts = testMailSender.Contacts;
            var actualMsg = testMailSender.Envelop;

            //Assert
            Assert.NotNull(actualContacts);
            Assert.Equal(2, actualContacts.Length);
            Assert.Equal("qux2@host.ru", actualContacts[0]);
            Assert.Equal("qux3@host.ru", actualContacts[1]);

            Assert.NotNull(actualMsg);
            Assert.Equal("bar", actualMsg.Subject);
            Assert.Equal("baz", actualMsg.Body);
        }

        class TestEmailSender : IEmailSender
        {
            public EmailEnvelop Envelop { get; set; }

            public string[] Contacts { get; set; }

            public Task SendNotificationAsync(string[] contacts, EmailEnvelop envelop)
            {
                Contacts = contacts;
                Envelop = envelop;

                return Task.CompletedTask;
            }
        }
    }
}
