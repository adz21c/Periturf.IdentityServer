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

namespace Periturf.IdentityServer.Tests.Configuration.ConfigurationStoreTests
{
    class IdentityResourceTests
    {
        private const string NoResource = "NoResource";
        private const string NoResourceScope = "NoResourceScope";
        private ConfigurationStore _configStore;

        private readonly IdentityResource _identityResource1 = new IdentityResource
        {
            Name = "Resource1",
            DisplayName = "R1",
            Enabled = true,
        };

        private readonly IdentityResource _identityResource1Alt = new IdentityResource
        {
            Name = "Resource1",
            DisplayName = "R1.5",
            Enabled = true,
        };

        private readonly IdentityResource _identityResource2 = new IdentityResource
        {
            Name = "Resource2",
            Enabled = false,
        };

        private readonly IdentityResource _identityResource3 = new IdentityResource
        {
            Name = "Resource3",
            Enabled = true,
        };

        [SetUp]
        public async Task SetUpAsync()
        {
            _configStore = new ConfigurationStore();
            var spec1 = new IdentityServerConfigurationSpecification(_configStore);
            spec1.IdentityResource(_identityResource1);
            spec1.IdentityResource(_identityResource2);
            await spec1.ApplyAsync();
        }

        [Test]
        public async Task Given_Resource1AndResource2_When_FindResourcesByScopes_Then_ResourcesFound()
        {
            var resourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _identityResource1.Name, _identityResource2.Name, NoResourceScope });
            var resources = resourcesEnum.ToList();
            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(resources.SingleOrDefault(x => x.Name == _identityResource1.Name), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Name == _identityResource2.Name), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Name == NoResourceScope), Is.Null);
        }
    }
}
