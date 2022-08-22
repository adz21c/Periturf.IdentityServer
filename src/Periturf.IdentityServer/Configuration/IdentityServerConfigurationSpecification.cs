﻿using Duende.IdentityServer.Models;
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
        private readonly List<ApiResource> _apiResources = new List<ApiResource>();

        public IdentityServerConfigurationSpecification(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        internal void Client(Client client)
        {
            _clients.Add(client);
        }

        public void ApiResource(ApiResource resource1)
        {
            _apiResources.Add(resource1);
        }

        public async Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            return await _configStore.AddConfigurationAsync(
                new Configuration(_clients, _apiResources),
                ct);
        }
    }
}
