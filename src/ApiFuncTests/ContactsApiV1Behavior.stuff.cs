using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.DbTest;
using MyLab.Notifier;
using MyLab.Notifier.Client;
using MyLab.Notifier.Client.Models;
using MyLab.Notifier.Share.Dal;
using Xunit.Abstractions;

namespace ApiFuncTests
{
    public partial class ContactsApiV1Behavior
    {
        private readonly TestApi<Program, INotifierContactApiV1> _api;
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<InitialDbInitializer> _db;

        /// <summary>
        /// Initializes a new instance of <see cref="ContactsApiV1Behavior"/>
        /// </summary>
        public ContactsApiV1Behavior(ITestOutputHelper output,
            TestApi<Program, INotifierContactApiV1> api,
            TmpDbFixture<InitialDbInitializer> db)
        {
            _output = output;
            _db = db;
            _db.Output = output;

            _api = api;
            _api.Output = output;
            _api.ServiceOverrider = s => s
                .AddLogging(l => l
                    .AddXUnit(output)
                    .AddFilter(_ => true)
                )
                .AddRabbitEmulation();
        }

        public static IEnumerable<object[]> GetBadCreateContactRequests()
        {
            var validContact = new ContactContentDto
            {
                ChannelId = "foo-channel",
                Value = "foo@host.com"
            };

            return new[]
            {
                new object[] { "Null subject", null, validContact },
                new object[] { "Empty subject", "", validContact },
                new object[] { "Whitespace subject", " \t", validContact },
                new object[] { "Null contact", "foo", null },
                new object[] { "Empty contact", "foo", new ContactContentDto() },
                new object[] { "Undefined value contact", "foo", new ContactContentDto { ChannelId = "foo-channel" } },
                new object[] { "Undefined channel contact", "foo", new ContactContentDto { Value = "foo@host.com" } }
            };
        }

        public class InitialDbInitializer : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.CreateTableAsync<ContactDb>();
                await dataConnection.CreateTableAsync<ContactLabelDb>();
                await dataConnection.CreateTableAsync<TopicBindingDb>();
            }
        }

        private class AddInitialContactDbIniter : ITestDbInitializer
        {
            private readonly ContactDb _contact;

            public int ContactId { get; private set; }

            public AddInitialContactDbIniter(ContactDb contact)
            {
                _contact = contact;
            }

            public async Task InitializeAsync(DataConnection dataConnection)
            {
                ContactId = (int)(long)await dataConnection.InsertWithIdentityAsync(_contact);

                if (_contact.Labels != null)
                {
                    await dataConnection.BulkCopyAsync(_contact.Labels.Select(l => new ContactLabelDb
                    {
                        ContactId = ContactId,
                        Value = l.Value,
                        Name = l.Name
                    }));
                }
            }
        }
    }
}