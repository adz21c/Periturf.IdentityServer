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
    class ApiResourceTests
    {
        private const string NoResource = "NoResource";
        private const string NoResourceScope = "NoResourceScope";
        private const string Scope1 = "Scope1";
        private const string Scope2 = "Scope2";
        private const string Scope3 = "Scope3";
        private ConfigurationStore _configStore;

        private readonly ApiResource _resource1 = new ApiResource
        {
            Name = "Resource1",
            DisplayName = "R1",
            Enabled = true,
            Scopes = new List<string> { Scope1 }
        };

        private readonly ApiResource _resource1Alt = new ApiResource
        {
            Name = "Resource1",
            DisplayName = "R1.5",
            Enabled = true,
            Scopes = new List<string> { Scope1 }
        };

        private readonly ApiResource _resource2 = new ApiResource
        {
            Name = "Resource2",
            Enabled = false,
            Scopes = new List<string> { Scope2 }
        };

        private readonly ApiResource _resource3 = new ApiResource
        {
            Name = "Resource3",
            Enabled = true,
            Scopes = new List<string> { Scope3 }
        };

        [SetUp]
        public async Task SetUpAsync()
        {
            _configStore = new ConfigurationStore();
            var spec1 = new IdentityServerConfigurationSpecification(_configStore);
            spec1.ApiResource(_resource1);
            spec1.ApiResource(_resource2);
            await spec1.ApplyAsync();
        }

        [Test]
        public async Task Given_Resource1AndResource2_When_FindResourcesByName_Then_ResourcesFound()
        {
            var resourcesEnum = await _configStore.FindApiResourcesByNameAsync(new[] { _resource1.Name, _resource2.Name, NoResource });
            var resources = resourcesEnum.ToList();
            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(2));

            var first = resources.SingleOrDefault(x => x.Name == _resource1.Name);
            Assert.That(first, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(first.Name, Is.EqualTo(_resource1.Name));
                Assert.That(first.Enabled, Is.EqualTo(_resource1.Enabled));
            });

            var last = resources.SingleOrDefault(x => x.Name == _resource2.Name);
            Assert.That(last, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(last.Name, Is.EqualTo(_resource2.Name));
                Assert.That(last.Enabled, Is.EqualTo(_resource2.Enabled));
            });

            var nothing = resources.SingleOrDefault(x => x.Name == NoResource);
            Assert.That(nothing, Is.Null);
        }

        [Test]
        public async Task Given_Resource1AndResource2_When_FindResourcesByScopes_Then_ResourcesFound()
        {
            var resourcesEnum = await _configStore.FindApiResourcesByScopeNameAsync(new[] { Scope1, Scope2, NoResourceScope });
            var resources = resourcesEnum.ToList();
            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(resources.SingleOrDefault(x => x.Scopes.Contains(Scope1)), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Scopes.Contains(Scope2)), Is.Not.Null);
            Assert.That(resources.SingleOrDefault(x => x.Scopes.Contains(NoResourceScope)), Is.Null);
        }
    }
}
