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
    class IdentityServerConfigurationSpecification : IConfigurationSpecification
    {
        private readonly ConfigurationStore _configStore;

        private readonly List<Client> _clients = new List<Client>();

        public IdentityServerConfigurationSpecification(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        internal void Client(Client client)
        {
            _clients.Add(client);
        }

        public async Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            return await _configStore.AddConfigurationAsync(
                _clients,
                ct);
        }
    }
}
