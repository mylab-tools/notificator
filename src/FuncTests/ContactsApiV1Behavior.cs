using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using MyLab.Db;
using MyLab.DbTest;
using MyLab.Notifier;
using MyLab.Notifier.Client;
using MyLab.Notifier.Client.Models;
using MyLab.Notifier.Dal;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public partial class ContactsApiV1Behavior : 
        IClassFixture<TestApi<Program, INotifierContactApiV1>>,
        IClassFixture<TmpDbFixture<ContactsApiV1Behavior.InitialDbInitializer>>
    {
        [Fact]
        public async Task ShouldCreateContact()
        {
            //Arrange
            var testDb = await _db.CreateDbAsync();

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            var contact = new ContactContent()
            {
                ChannelId = "foo-channel",
                Value = "foo@host.com",
                Labels = new Dictionary<string, string>
                {
                    {"foo-key", "foo-value"}
                }
            };

            //Act
            var contactId = await api.AddContactAsync("foo-subject", contact);

            await using var db = testDb.Use();

            var contactFromDb = await db.Tab<ContactDb>()
                .LoadWith(c => c.Labels)
                .FirstOrDefaultAsync(c => c.Id == contactId);

            //Assert
            Assert.NotNull(contactFromDb);
            Assert.Equal("foo-channel", contactFromDb.ChannelId);
            Assert.Equal("foo-subject", contactFromDb.SubjectId);
            Assert.Equal("foo@host.com", contactFromDb.Value);
            Assert.Equal(contactId, contactFromDb.Id);
            Assert.NotNull(contactFromDb.Labels);
            Assert.Single(contactFromDb.Labels);
            Assert.Equal("foo-key", contactFromDb.Labels?[0].Name);
            Assert.Equal("foo-value", contactFromDb.Labels?[0].Value);
        }

        [Theory]
        [MemberData(nameof(GetBadCreateContactRequests))]
        public async Task ShouldFailWhenAddContactBadRequest(string desc, string subjectId, ContactContent contact)
        {
            //Arrange
            var testDb = await _db.CreateDbAsync();

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            ResponseCodeException requestException = null;

            //Act
            try
            {
                await api.AddContactAsync(subjectId, contact);
            }
            catch (ResponseCodeException e)
            {
                requestException = e;

                _output.WriteLine("Response: " + e.ServerMessage);
            }

            //Assert
            Assert.NotNull(requestException);
            Assert.Equal(HttpStatusCode.BadRequest, requestException.StatusCode);
        }

        [Fact]
        public async Task ShouldGetContacts()
        {
            //Arrange
            var contact = new ContactDb
            {
                ChannelId = "foo-channel",
                Value = "foo@host.com",
                Labels = new[]
                {
                    new ContactLabelDb{ Name = "foo-key", Value = "foo-value"}
                }
            };

            var dbInitiator = new AddInitialContact(contact);
            var testDb = await _db.CreateDbAsync(dbInitiator);

            var contactId = dbInitiator.ContactId;

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            //Act
            var contacts = await api.GetContactsAsync("foo-subject");


            //Assert
            Assert.NotNull(contacts);
            Assert.Single(contacts);
            Assert.Equal("foo-channel", contacts[0].ChannelId);
            Assert.Equal("foo@host.com", contacts[0].Value);
            Assert.Equal(contactId, contacts[0].Id);
            Assert.NotNull(contacts[0].Labels);
            Assert.Single(contacts[0].Labels);
            Assert.True(contacts[0].Labels.TryGetValue("foo-key", out var expectedLabelValue));
            Assert.Equal("foo-value", expectedLabelValue);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t")]
        public async Task ShouldFailWhenGetContactsWithUndefinedSubjectId(string invalidSubjectId)
        {
            //Arrange
            var testDb = await _db.CreateDbAsync();

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            ResponseCodeException requestException = null;

            //Act
            try
            {
                await api.GetContactsAsync(invalidSubjectId);
            }
            catch (ResponseCodeException e)
            {
                requestException = e;

                _output.WriteLine("Response: " + e.ServerMessage);
            }

            //Assert
            Assert.NotNull(requestException);
            Assert.Equal(HttpStatusCode.BadRequest, requestException.StatusCode);
        }

        [Fact]
        public async Task ShouldDeleteContact()
        {
            //Arrange
            var contact = new ContactDb
            {
                ChannelId = "foo-channel",
                Value = "foo@host.com",
            };

            var dbInitiator = new AddInitialContact(contact);
            var testDb = await _db.CreateDbAsync(dbInitiator);

            var contactId = dbInitiator.ContactId;

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));
            
            //Act
            await api.DeleteContactAsync(contactId);

            var exists = await testDb.DoOnce().Tab<ContactDb>().AnyAsync(c => c.Id == contactId);

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldGetContact()
        {
            //Arrange
            var initialContact = new ContactDb
            {
                ChannelId = "foo-channel",
                Value = "foo@host.com",
                Labels = new[]
                {
                    new ContactLabelDb{ Name = "foo-key", Value = "foo-value"}
                }
            };

            var dbInitiator = new AddInitialContact(initialContact);
            var testDb = await _db.CreateDbAsync(dbInitiator);

            var contactId = dbInitiator.ContactId;

            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            //Act
            var contact = await api.GetContactAsync(contactId);

            //Assert
            Assert.NotNull(contact);
            Assert.Equal("foo-channel", contact.ChannelId);
            Assert.Equal("foo@host.com", contact.Value);
            Assert.Equal(contactId, contact.Id);
            Assert.NotNull(contact.Labels);
            Assert.Single(contact.Labels);
            Assert.True(contact.Labels.TryGetValue("foo-key", out var expectedLabelValue));
            Assert.Equal("foo-value", expectedLabelValue);
        }

        [Fact]
        public async Task ShouldNotFoundWhenContactDoesNotExist()
        {
            //Arrange
            var testDb = await _db.CreateDbAsync();
            var api = _api.StartWithProxy(s => s.AddSingleton(testDb));

            ResponseCodeException requestException = null;

            //Act
            try
            {
                await api.GetContactAsync(123);
            }
            catch (ResponseCodeException codeException)
            {
                _output.WriteLine(codeException.ServerMessage);
                requestException = codeException;
            }

            //Assert
            Assert.NotNull(requestException);
            Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);
        }
    }
}