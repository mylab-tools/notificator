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
using Xunit.Abstractions;

namespace ApiFuncTests
{
    public partial class SubjectsApiV1Behavior
    {
        private readonly TestApi<Program, INotifierSubjectApiV1> _api;
        private readonly ITestOutputHelper _output;
        private readonly TmpDbFixture<InitialDbInitializer> _db;

        public SubjectsApiV1Behavior(ITestOutputHelper output,
            TestApi<Program, INotifierSubjectApiV1> api,
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

        public class InitialDbInitializer : ITestDbInitializer
        {
            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.CreateTableAsync<TopicBindingDb>();
            }
        }

        private class AddInitialBindingInitializer : ITestDbInitializer
        {
            private readonly TopicBindingDb _binding;

            public AddInitialBindingInitializer(TopicBindingDb binding)
            {
                _binding = binding;
            }

            public async Task InitializeAsync(DataConnection dataConnection)
            {
                await dataConnection.InsertAsync(_binding);
            }
        }
    }
}