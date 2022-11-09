using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Test;
using MyLab.Db;
using MyLab.DbTest;
using MyLab.Notifier;
using MyLab.Notifier.Client;
using MyLab.Notifier.Share.Dal;
using Xunit;

namespace FuncTests
{
    public partial class SubjectsApiV1Behavior : 
        IClassFixture<TestApi<Program, INotifierSubjectApiV1>>,
        IClassFixture<TmpDbFixture<SubjectsApiV1Behavior.InitialDbInitializer>>
    {
        [Fact]
        public async Task ShouldBind()
        {
            //Arrange
            var testDb = await _db.CreateDbAsync();

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            //Act
            await api.BindTopicAsync("foo-subject", "foo-topic");

            await using var db = testDb.Use();

            var exists = await db.Tab<TopicBindingDb>()
                .AnyAsync(b => b.SubjectId == "foo-subject" && b.TopicId == "foo-topic");

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldUnbind()
        {
            //Arrange
            var initialBinding = new TopicBindingDb
            {
                SubjectId = "foo-subject",
                TopicId = "foo-topic"
            };

            var dbIniter = new AddInitialBindingInitializer(initialBinding);

            var testDb = await _db.CreateDbAsync(dbIniter);

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            //Act
            await api.UnbindTopicAsync("foo-subject", "foo-topic");

            await using var db = testDb.Use();

            var exists = await db.Tab<TopicBindingDb>()
                .AnyAsync(b => b.SubjectId == "foo-subject" && b.TopicId == "foo-topic");

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldProvideSubjectTopics()
        {
            //Arrange
            var initialBinding = new TopicBindingDb
            {
                SubjectId = "foo-subject",
                TopicId = "foo-topic"
            };

            var dbIniter = new AddInitialBindingInitializer(initialBinding);

            var testDb = await _db.CreateDbAsync(dbIniter);

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            //Act
            var topics = await api.GetSubjectTopicsAsync("foo-subject");

            //Assert
            Assert.NotNull(topics);
            Assert.Single(topics);
            Assert.Equal("foo-topic", topics[0]);
        }
    }
}