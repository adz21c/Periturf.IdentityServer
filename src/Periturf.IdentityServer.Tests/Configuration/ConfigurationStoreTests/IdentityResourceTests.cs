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

        private readonly IdentityResource _resource1 = new IdentityResource
        {
            Name = "Resource1",
            DisplayName = "R1",
            Enabled = true,
        };

        private readonly IdentityResource _resource1Alt = new IdentityResource
        {
            Name = "Resource1",
            DisplayName = "R1.5",
            Enabled = true,
        };

        private readonly IdentityResource _resource2 = new IdentityResource
        {
            Name = "Resource2",
            Enabled = false,
        };

        private readonly IdentityResource _resource3 = new IdentityResource
        {
            Name = "Resource3",
            Enabled = true,
        };

        [SetUp]
        public async Task SetUpAsync()
        {
            _configStore = new ConfigurationStore();
            var spec1 = new IdentityServerConfigurationSpecification(_configStore);
            spec1.IdentityResource(_resource1);
            spec1.IdentityResource(_resource2);
            await spec1.ApplyAsync();
        }

        [Test]
        public async Task Given_Resource1AndResource2_When_FindResourcesByScopes_Then_ResourcesFound()
        {
            var resourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _resource1.Name, _resource2.Name, NoResourceScope });
            var resources = resourcesEnum.ToList();
            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(resources.SingleOrDefault(x => x.Name == _resource1.Name), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Name == _resource2.Name), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Name == NoResourceScope), Is.Null);
        }

        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_FindResourcesByScopes_Then_R2AndR3FoundAndR1AltFound()
        {
            var initialResourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _resource1.Name, _resource2.Name, _resource3.Name, NoResourceScope });
            var initialResources = initialResourcesEnum.ToList(); 
            Assume.That(initialResources, Is.Not.Null.And.Count.EqualTo(2));
            Assume.That(initialResources.SingleOrDefault(x => x.Name == _resource1.Name), Is.Not.Null);
            Assume.That(initialResources.SingleOrDefault(x => x.Name == _resource2.Name), Is.Not.Null);
            Assume.That(initialResources.SingleOrDefault(x => x.Name == _resource3.Name), Is.Null);
            Assume.That(initialResources.SingleOrDefault(x => x.Name == NoResourceScope), Is.Null);

            var spec2 = new IdentityServerConfigurationSpecification(_configStore);
            spec2.IdentityResource(_resource1Alt);
            spec2.IdentityResource(_resource3);
            await spec2.ApplyAsync();

            var resourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _resource1.Name, _resource2.Name, _resource3.Name, NoResource });
            var resources = resourcesEnum.ToList();

            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(3));
            var r1 = resources.SingleOrDefault(x => x.Name == _resource1.Name);
            Assert.That(r1, Is.Not.Null);
            Assert.That(r1.DisplayName, Is.EqualTo(_resource1Alt.DisplayName));
            Assert.That(resources.SingleOrDefault(x => x.Name == _resource2.Name), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Name == NoResourceScope), Is.Null);
        }
    }
}
