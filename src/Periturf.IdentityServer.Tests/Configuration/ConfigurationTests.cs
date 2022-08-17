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
    class ConfigurationTests
    {
        [Test]
        public void Given_ConfigurationContext_When_IdentityServer_Then_NewConfigSpecificationAddedAsync()
        {
            var name = A.Dummy<string>();
            var _sut = new IdentityServerConfigurationSpecification(new ConfigurationStore());

            var configContext = A.Fake<IConfigurationContext>();
            A.CallTo(() => configContext.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(A<string>._)).Returns(_sut);

            configContext.IdentityServer(name);

            A.CallTo(() => configContext.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(name)).MustHaveHappened().Then(
                A.CallTo(() => configContext.AddSpecification(_sut)).MustHaveHappenedOnceExactly());
        }

        [Test]
        public async Task Given_ConfigurationStore_When_AddClient_Then_FindClientLocates()
        {
            var configStore = new ConfigurationStore();
            var _sut = new IdentityServerConfigurationSpecification(configStore);

            var secret = A.Dummy<string>().Sha256();
            var client = new Client
            {
                ClientId = A.Dummy<string>(),
                ClientSecrets = new List<Secret>() { new Secret(secret) },
                Enabled = true
            };

            _sut.Client(client);

            await _sut.ApplyAsync();

            var foundClient = await configStore.FindClientByIdAsync(client.ClientId);
            Assert.That(foundClient, Is.Not.Null);
            Assert.That(foundClient.ClientId, Is.EqualTo(client.ClientId));
            Assert.That(foundClient.ClientSecrets, Is.Not.Empty.And.Count.EqualTo(1).And.One.With.Property("Value").EqualTo(secret));
            Assert.That(foundClient.Enabled, Is.EqualTo(client.Enabled));
        }
    }
}
