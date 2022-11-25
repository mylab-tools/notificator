using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MyLab.Notifier.ChannelAdapter;
using MyLab.Notifier.EmailSender;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Connection;
using MyLab.RabbitClient.Model;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.EmailSender
{
    public class EmailSenderBehavior : IDisposable
    {
        private const string EmailHost = "localhost";
        private const string EmailUser = "test-login";
        private const string EmailPassword = "test-pass";
        private const int SmtpPort = 10162;
        private const int ImapPort = 10163;

        private const string RabbitHost = "localhost";
        private const int RabbitPort = 10160;

        private readonly ITestOutputHelper _output;
        private readonly RabbitQueueFactory _mqFactory;
        private readonly ImapClient _imapClient;

        /// <summary>
        /// Initializes a new instance of <see cref="EmailSenderBehavior"/>
        /// </summary>
        public EmailSenderBehavior(ITestOutputHelper output)
        {
            _output = output;
            var rabbitOptions = new RabbitOptions
            {
                Host = EmailHost,
                Port = RabbitPort
            };

            var rabbitConnectionProvider = new LazyRabbitConnectionProvider(rabbitOptions);
            var rabbitChannelProvider = new RabbitChannelProvider(rabbitConnectionProvider);

            _mqFactory = new RabbitQueueFactory(rabbitChannelProvider)
            {
                AutoDelete = true
            };

            _imapClient = new ImapClient();
        }

        [Fact]
        public async Task ShouldSendEmailToContacts()
        {
            //Arrange
            var queue = _mqFactory.CreateWithRandomId();

            using var queueRemover = new QueueRemover(queue);

            var services = new ServiceCollection()
                .AddMailSenderLogic()
                .ConfigureRabbit(opt =>
                {
                    opt.Host = RabbitHost;
                    opt.Port = RabbitPort;
                })
                .ConfigureNotifierChannelLogic(opt => opt.MqQueue = queue.Name) 
                .ConfigureMailSender(emailChannelOpt =>
                {
                    //default
                }, smtpOpt =>
                {
                    smtpOpt.Host = EmailHost;
                    smtpOpt.Username = EmailUser;
                    smtpOpt.Password = EmailPassword;
                    smtpOpt.Port = SmtpPort;
                })
                .AddLogging(l => l
                    .ClearProviders()
                    .AddFilter(l => true)
                    .AddXUnit(_output)
                )
                .BuildServiceProvider();

            var mqNotification = new EnvelopMqDto
            {
                SendNotificationCmd = new SendNotificationMqDto
                {
                    Contacts = new []
                    {
                        "foo@host.com",
                        "bar@host.com"
                    },
                    Notification = new NotificationDto
                    {
                        Title = "baz",
                        Body = "qoz"
                    }
                }
            };

            var hostedSrv = services.GetServices<IHostedService>();

            await using var hostedSrvController = new HostedServicesController(hostedSrv);
            
            //Act
            queue.Publish(mqNotification);

            var message = await ReadEmailAsync();

            //Assert
            Assert.NotNull(message);

        }

        async Task<MimeMessage> ReadEmailAsync()
        {
            await _imapClient.ConnectAsync(EmailHost, ImapPort);
            await _imapClient.AuthenticateAsync(EmailUser, EmailPassword);

            var inbox = _imapClient.Inbox;

            await inbox.OpenAsync(FolderAccess.ReadOnly);

            return await inbox.GetMessageAsync(0);
        }

        class HostedServicesController : IAsyncDisposable
        {
            private readonly IHostedService[] _hostedServices;

            public HostedServicesController(IEnumerable<IHostedService> hostedServices)
            {
                _hostedServices = hostedServices.ToArray();
            }

            public async Task StartAsync()
            {
                foreach (var hostedService in _hostedServices)
                {
                    await hostedService.StartAsync(CancellationToken.None);
                }
            }

            public async ValueTask DisposeAsync()
            {
                foreach (var hostedService in _hostedServices)
                {
                    await hostedService.StopAsync(CancellationToken.None);
                }
            }
        }

        class QueueRemover : IDisposable
        {
            private readonly RabbitQueue _queue;

            public QueueRemover(RabbitQueue queue)
            {
                _queue = queue;
            }

            public void Dispose()
            {
                _queue.Remove();
            }
        }

        public void Dispose()
        {
            _imapClient?.Disconnect(true);
            _imapClient?.Dispose();
        }
    }
}
