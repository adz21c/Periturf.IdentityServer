using Duende.IdentityServer.Models;
using Periturf.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    class IdentityServerConfigurationSpecification : IConfigurationSpecification, IIdentityServerConfigurationConfigurator
    {
        private readonly ConfigurationStore _configStore;

        private readonly List<Client> _clients = new List<Client>();
        private readonly List<ApiResource> _apiResources = new List<ApiResource>();
        private readonly List<IdentityResource> _identityResources = new List<IdentityResource>();
        private readonly List<ApiScope> _apiScopes = new List<ApiScope>();

        public IdentityServerConfigurationSpecification(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        public void Client(Client client)
        {
            _clients.Add(client);
        }

        public void ApiResource(ApiResource resource)
        {
            _apiResources.Add(resource);
        }

        public void IdentityResource(IdentityResource resource)
        {
            _identityResources.Add(resource);
        }

        public void Scope(ApiScope scope)
        {
            _apiScopes.Add(scope);
        }

        public async Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            return await _configStore.AddConfigurationAsync(
                new Configuration(_clients, _apiResources, _identityResources, _apiScopes),
                ct);
        }
    }
}
