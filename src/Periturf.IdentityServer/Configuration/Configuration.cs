using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Periturf.IdentityServer.Configuration
{
    class Configuration
    {
        public Configuration(List<Client>? clients, List<ApiResource>? apiResources)
        {
            Clients = clients;
            ApiResources = apiResources;
        }

        public List<Client>? Clients { get; }

        public List<ApiResource>? ApiResources { get; }
    }
}