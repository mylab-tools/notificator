using System;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using MyLab.Notifier;
using MyLab.Notifier.Api;
using MyLab.Notifier.Client;
using MyLab.Notifier.Share.Dal;
using MyLab.Notifier.Share.Models;
using MyLab.RabbitClient;
using MyLab.RabbitClient.Consuming;
using Xunit.Abstractions;
using NotificationDto = MyLab.Notifier.Client.Models.NotificationDto;

namespace ApiFuncTests
{
    public partial class NotificationsApiV1Behavior
    {
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<InitialDbInitializer> _db;
        private readonly TestApi<Program, INotifierNotificationsApiV1> _api;

        public NotificationsApiV1Behavior(ITestOutputHelper output,
            TestApi<Program, INotifierNotificationsApiV1> api,
            TmpDbFixture<InitialDbInitializer> db)
        {
            _output = output;
            _db = db;
            _db.Output = output;

            _api = api;
            _api.Output = output;
            _api.ServiceOverrider = s =>
                s.AddLogging(l => l.AddXUnit(output).AddFilter(_ => true))
                    .ConfigureRabbit(opt =>
                        opt.DefaultPub = new PublishOptions
                        {
                            Exchange = "foo-exchange"
                        }
                    );
        }

        public class AddContactsDbIniter : ITestDbInitializer
        {
            private readonly ContactDb[] _contacts;

            public AddContactsDbIniter(params ContactDb[] contacts)
            {
                _contacts = contacts;
            }

            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.BulkCopyAsync(_contacts);
            }
        }

        public class InitialDbInitializer : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.CreateTableAsync<ContactDb>();
            }
        }

        private class TestMqConsumer : RabbitConsumer<EnvelopMqDto>
        {
            public EnvelopMqDto LastMessage { get; private set; }

            protected override async Task ConsumeMessageAsync(ConsumedMessage<EnvelopMqDto> consumedMessage)
            {
                LastMessage = consumedMessage.Content;
            }
        }

        private NotificationDto GenerateNotification() => new NotificationDto
        {
            Title = Guid.NewGuid().ToString("N"),
            Body = Guid.NewGuid().ToString("N"),
        };
    }
}