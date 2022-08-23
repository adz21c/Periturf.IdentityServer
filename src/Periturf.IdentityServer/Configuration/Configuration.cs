using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Periturf.IdentityServer.Configuration
{
    class Configuration
    {
        public Configuration(List<Client>? clients, List<ApiResource>? apiResources, List<IdentityResource> identityResources)
        {
            Clients = clients;
            ApiResources = apiResources;
            IdentityResources = identityResources;
        }

        public List<Client>? Clients { get; }

        public List<ApiResource>? ApiResources { get; }

        public List<IdentityResource>? IdentityResources { get; }
    }
}