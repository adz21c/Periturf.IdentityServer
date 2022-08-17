using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Periturf.IdentityServer.Configuration
{
    class Configuration
    {
        public Configuration(List<Client> clients)
        {
            Clients = clients;
        }

        public List<Client> Clients { get; }
    }
}