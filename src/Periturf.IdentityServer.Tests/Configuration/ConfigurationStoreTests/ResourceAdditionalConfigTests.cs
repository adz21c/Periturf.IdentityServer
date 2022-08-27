using Duende.IdentityServer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
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

        private readonly ApiScope _scope1 = new ApiScope(Scope1, "Scope1");
        private readonly ApiScope _scope1Alt = new ApiScope(Scope1, "Scope1Alt");
        private readonly ApiScope _scope2 = new ApiScope(Scope2);
        private readonly ApiScope _scope3 = new ApiScope(Scope3);

        [SetUp]
        public async Task SetUpAsync()
        {
            _configStore = new ConfigurationStore();
            var spec1 = new IdentityServerConfigurationSpecification(_configStore);
            spec1.ApiResource(_resource1);
            spec1.ApiResource(_resource2);
            spec1.IdentityResource(_identityResource1);
            spec1.IdentityResource(_identityResource2);
            spec1.Scope(_scope1);
            spec1.Scope(_scope2);
            await spec1.ApplyAsync();

            var spec2 = new IdentityServerConfigurationSpecification(_configStore);
            spec2.ApiResource(_resource1Alt);
            spec2.ApiResource(_resource3);
            spec2.IdentityResource(_identityResource1Alt);
            spec2.IdentityResource(_identityResource3);
            spec2.Scope(_scope1Alt);
            spec2.Scope(_scope3);
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
            Assert.That(resources, Has.One.Property("Name").EqualTo(_identityResource2.Name));
            Assert.That(resources, Has.One.Property("Name").EqualTo(_identityResource3.Name));
            Assert.That(resources, Has.None.Property("Name").EqualTo(NoResource));
        }

        [Test]
        public async Task Given_ConfigWithR1R2AndConfigWithR1AltAndR3_When_FindApiScopesByName_Then_R2AndR3FoundAndR1AltFound()
        {
            var scopesEnum = await _configStore.FindApiScopesByNameAsync(new[] { _scope1.Name, _scope2.Name, _scope3.Name, NoResourceScope });
            var scopes = scopesEnum.ToList();

            Assert.That(scopes, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(scopes, Has.One.Property("Name").EqualTo(_scope1.Name).And.Property("DisplayName").EqualTo(_scope1Alt.DisplayName));
            Assert.That(scopes, Has.One.Property("Name").EqualTo(_scope2.Name));
            Assert.That(scopes, Has.One.Property("Name").EqualTo(_scope3.Name));
            Assert.That(scopes, Has.None.Property("Name").EqualTo(NoResource));
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
            Assert.That(resources.IdentityResources, Has.One.Property("Name").EqualTo(_identityResource2.Name));
            Assert.That(resources.IdentityResources, Has.One.Property("Name").EqualTo(_identityResource3.Name));
            Assert.That(resources.IdentityResources, Has.None.Property("Name").EqualTo(NoResource));

            Assert.That(resources?.ApiScopes, Is.Not.Null.And.Count.EqualTo(3));
            Assert.That(resources.ApiScopes, Has.One.Property("Name").EqualTo(_scope1.Name).And.Property("DisplayName").EqualTo(_scope1Alt.DisplayName));
            Assert.That(resources.ApiScopes, Has.One.Property("Name").EqualTo(_scope2.Name));
            Assert.That(resources.ApiScopes, Has.One.Property("Name").EqualTo(_scope3.Name));
            Assert.That(resources.ApiScopes, Has.None.Property("Name").EqualTo(NoResource));
        }
    }
}
