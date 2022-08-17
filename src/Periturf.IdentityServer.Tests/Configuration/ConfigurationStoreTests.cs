using Duende.IdentityServer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Periturf.Configuration;
using Periturf.IdentityServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Tests.Configuration
{
    class ConfigurationStoreTests
    {
        private ConfigurationStore _configStore;
        
        private string _secret1 = "Secret".Sha256();
        private Client _client1 = new Client
        {
            ClientId = "ClientId",
            ClientName = "ClientName",
            ClientSecrets = new List<Secret>(),
            Enabled = true
        };

        private string _secret2 = "Secret2".Sha256();
        private Client _client2 = new Client
        {
            ClientId = "ClientId2",
            ClientSecrets = new List<Secret>(),
            Enabled = true
        };

        private string _secret3 = "Secret3".Sha256();
        private Client _client3 = new Client
        {
            ClientId = "ClientId3",
            ClientSecrets = new List<Secret>(),
            Enabled = true
        };

        private Client _client1Alt = new Client();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _client1.ClientSecrets.Add(new Secret(_secret1));
            _client2.ClientSecrets.Add(new Secret(_secret2));
            _client3.ClientSecrets.Add(new Secret(_secret3));

            _client1Alt.ClientId = _client1.ClientId;
            _client1Alt.ClientName = _client1.ClientName + "Alt";
            _client1Alt.ClientSecrets.Add(new Secret(_secret1));
        }

        [SetUp]
        public async Task SetUpAsync()
        {
            _configStore = new ConfigurationStore();
            var spec1 = new IdentityServerConfigurationSpecification(_configStore);
            spec1.Client(_client1);
            await spec1.ApplyAsync();
        }

        [Test]
        public async Task Given_Client1_When_FindClient1_Then_ClientFound()
        {
            var foundClient = await _configStore.FindClientByIdAsync(_client1.ClientId);
            Assert.That(foundClient, Is.Not.Null);
            Assert.That(foundClient.ClientId, Is.EqualTo(_client1.ClientId));
            Assert.That(foundClient.ClientSecrets, Is.Not.Empty.And.Count.EqualTo(1).And.One.With.Property("Value").EqualTo(_secret1));
            Assert.That(foundClient.Enabled, Is.EqualTo(_client1.Enabled));
        }

        [Test]
        public async Task Given_Client1_When_FindClient2_Then_ClientNotFound()
        {
            var foundClient = await _configStore.FindClientByIdAsync(_client2.ClientId);
            Assert.That(foundClient, Is.Null);
        }

        [Test]
        public async Task Given_Config1WithClient1AndConfig2WithClients2And3_When_FindClient2And3_Then_ClientsFound()
        {
            var spec2 = new IdentityServerConfigurationSpecification(_configStore);
            spec2.Client(_client2);
            spec2.Client(_client3);
            await spec2.ApplyAsync();

            Client? client1 = null;
            Assume.That(async () => client1 = await _configStore.FindClientByIdAsync(_client1.ClientId), Throws.Nothing);
            Assume.That(client1, Is.Not.Null);
            Assume.That(client1.ClientName, Is.EqualTo(_client1.ClientName));

            var foundClient2 = await _configStore.FindClientByIdAsync(_client2.ClientId);
            Assert.That(foundClient2, Is.Not.Null);
            Assert.That(foundClient2.ClientId, Is.EqualTo(_client2.ClientId));
            Assert.That(foundClient2.ClientSecrets, Is.Not.Empty.And.Count.EqualTo(1).And.One.With.Property("Value").EqualTo(_secret2));
            Assert.That(foundClient2.Enabled, Is.EqualTo(_client2.Enabled));

            var foundClient3 = await _configStore.FindClientByIdAsync(_client3.ClientId);
            Assert.That(foundClient3, Is.Not.Null);
            Assert.That(foundClient3.ClientId, Is.EqualTo(_client3.ClientId));
            Assert.That(foundClient3.ClientSecrets, Is.Not.Empty.And.Count.EqualTo(1).And.One.With.Property("Value").EqualTo(_secret3));
            Assert.That(foundClient3.Enabled, Is.EqualTo(_client3.Enabled));
        }

        [Test]
        public async Task Given_Config1WithClient1AndConfig2WithClient1Alt_When_FindClient1_Then_Client1AltFound()
        {
            Client? client1 = null;
            Assume.That(async () => client1 = await _configStore.FindClientByIdAsync(_client1.ClientId), Throws.Nothing);
            Assume.That(client1, Is.Not.Null);
            Assume.That(client1.ClientName, Is.EqualTo(_client1.ClientName));

            var spec1Alt = new IdentityServerConfigurationSpecification(_configStore);
            spec1Alt.Client(_client1Alt);
            await spec1Alt.ApplyAsync();

            var foundClient = await _configStore.FindClientByIdAsync(_client1.ClientId);
            Assert.That(foundClient, Is.Not.Null);
            Assert.That(foundClient.ClientName, Is.EqualTo(_client1Alt.ClientName));
        }
    }
}
