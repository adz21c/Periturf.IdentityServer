using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Periturf.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    class ConfigurationStore : IClientStore
    {
        private readonly List<Configuration> _configurations = new List<Configuration>();

        public ConfigurationStore()
        {
        }

        internal Task<IConfigurationHandle> AddConfigurationAsync(List<Client> clients, CancellationToken ct)
        {
            var config = new Configuration(clients);
            _configurations.Add(config);
            return Task.FromResult<IConfigurationHandle>(new ConfigurationHandle(_configurations, config));
        }

        public Task<Client?> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.Clients ?? Enumerable.Empty<Client>())
                .FirstOrDefault(x => x.ClientId == clientId));
        }
    }
}