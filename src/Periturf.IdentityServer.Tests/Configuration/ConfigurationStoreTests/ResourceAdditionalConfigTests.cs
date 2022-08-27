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
    class ResourceAdditionalConfigTests
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
            spec1.ApiResource(_resource1);
            spec1.ApiResource(_resource2);
            spec1.IdentityResource(_identityResource1);
            spec1.IdentityResource(_identityResource2);
            await spec1.ApplyAsync();

            var initialApiResourcesEnum = await _configStore.FindApiResourcesByNameAsync(new[] { _resource1.Name, _resource2.Name, _resource3.Name, NoResource });
            var initialApiResources = initialApiResourcesEnum.ToList();
            Assume.That(initialApiResources, Is.Not.Null.And.Count.EqualTo(2));
            Assume.That(initialApiResources, Has.One.Property("Name").EqualTo(_resource1.Name));
            Assume.That(initialApiResources, Has.One.Property("Name").EqualTo(_resource2.Name));
            Assume.That(initialApiResources, Has.None.Property("Name").EqualTo(_resource3.Name));
            Assume.That(initialApiResources, Has.None.Property("Name").EqualTo(NoResource));

            var initialIdentityResourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _identityResource1.Name, _identityResource2.Name, _identityResource3.Name, NoResourceScope });
            var initialIdentityResources = initialIdentityResourcesEnum.ToList();
            Assume.That(initialIdentityResources, Is.Not.Null.And.Count.EqualTo(2));
            Assume.That(initialIdentityResources, Has.One.Property("Name").EqualTo(_resource1.Name));
            Assume.That(initialIdentityResources, Has.One.Property("Name").EqualTo(_resource2.Name));
            Assume.That(initialIdentityResources, Has.None.Property("Name").EqualTo(_resource3.Name));
            Assume.That(initialIdentityResources, Has.None.Property("Name").EqualTo(NoResource));

            var spec2 = new IdentityServerConfigurationSpecification(_configStore);
            spec2.ApiResource(_resource1Alt);
            spec2.ApiResource(_resource3);
            spec2.IdentityResource(_identityResource1Alt);
            spec2.IdentityResource(_identityResource3);
            await spec2.ApplyAsync();
        }


        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_FindApiResourcesByName_Then_R2AndR3FoundAndR1AltFound()
        {
            var resourcesEnum = await _configStore.FindApiResourcesByNameAsync(new[] { _resource1.Name, _resource2.Name, _resource3.Name, NoResource });
            var resources = resourcesEnum.ToList();

            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource1.Name).And.Property("DisplayName").EqualTo(_resource1Alt.DisplayName));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource2.Name));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource3.Name));
            Assert.That(resources, Has.None.Property("Name").EqualTo(NoResource));
        }


        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_FindApiResourcesByScopes_Then_R2AndR3FoundAndR1AltFound()
        {
            var resourcesEnum = await _configStore.FindApiResourcesByScopeNameAsync(new[] { Scope1, Scope2, Scope3, NoResourceScope });
            var resources = resourcesEnum.ToList();
            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource1.Name).And.Property("DisplayName").EqualTo(_resource1Alt.DisplayName));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource2.Name));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_resource3.Name));
            Assert.That(resources, Has.None.Property("Name").EqualTo(NoResource));
        }

        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_FindIdentityResourcesByScopes_Then_R2AndR3FoundAndR1AltFound()
        {
            var resourcesEnum = await _configStore.FindIdentityResourcesByScopeNameAsync(new[] { _identityResource1.Name, _identityResource2.Name, _identityResource3.Name, NoResource });
            var resources = resourcesEnum.ToList();

            Assert.That(resources, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_identityResource1.Name).And.Property("DisplayName").EqualTo(_identityResource1Alt.DisplayName));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_identityResource1.Name));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_identityResource1.Name));
            Assert.That(resources, Has.None.Property("Name").EqualTo(NoResource));
        }


        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_GetAllResources_Then_R2AndR3FoundAndR1AltFound()
        {
            var resources = await _configStore.GetAllResourcesAsync();

            Assert.That(resources?.ApiResources, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources.ApiResources, Has.One.Property("Name").EqualTo(_resource1.Name).And.Property("DisplayName").EqualTo(_resource1Alt.DisplayName));
            Assert.That(resources.ApiResources, Has.One.Property("Name").EqualTo(_resource2.Name));
            Assert.That(resources.ApiResources, Has.One.Property("Name").EqualTo(_resource3.Name));
            Assert.That(resources.ApiResources, Has.None.Property("Name").EqualTo(NoResource));

            Assert.That(resources?.IdentityResources, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources.IdentityResources, Has.One.Property("Name").EqualTo(_identityResource1.Name).And.Property("DisplayName").EqualTo(_identityResource1Alt.DisplayName));
            Assert.That(resources.IdentityResources, Has.One.Property("Name").EqualTo(_identityResource1.Name));
            Assert.That(resources.IdentityResources, Has.One.Property("Name").EqualTo(_identityResource1.Name));
            Assert.That(resources.IdentityResources, Has.None.Property("Name").EqualTo(NoResource));
        }
    }
}
