//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

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
    class ConfigurationStore : IClientStore, IResourceStore
    {
        private readonly List<Configuration> _configurations = new List<Configuration>();

        public ConfigurationStore()
        {
        }

        internal Task<IConfigurationHandle> AddConfigurationAsync(Configuration config, CancellationToken ct)
        {
            _configurations.Add(config);
            return Task.FromResult<IConfigurationHandle>(new ConfigurationHandle(_configurations, config));
        }

        public Task<Client?> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.Clients ?? Enumerable.Empty<Client>())
                .FirstOrDefault(x => x.ClientId == clientId));
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.IdentityResources ?? Enumerable.Empty<IdentityResource>())
                .Where(x => scopeNames.Contains(x.Name))
                .DistinctBy(x => x.Name));
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.ApiScopes ?? Enumerable.Empty<ApiScope>())
                .Where(x => scopeNames.Contains(x.Name))
                .DistinctBy(x => x.Name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.ApiResources ?? Enumerable.Empty<ApiResource>())
                .Where(x => x.Scopes.Intersect(scopeNames).Any())
                .DistinctBy(x => x.Name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return Task.FromResult(_configurations.Reverse<Configuration>()
                .SelectMany(x => x.ApiResources ?? Enumerable.Empty<ApiResource>())
                .Where(x => apiResourceNames.Contains(x.Name))
                .DistinctBy(x => x.Name));
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var configs = _configurations.Reverse<Configuration>().ToList();

            return Task.FromResult(new Resources(
                configs.SelectMany(x => x.IdentityResources ?? Enumerable.Empty<IdentityResource>())
                    .DistinctBy(x => x.Name),
                configs.SelectMany(x => x.ApiResources ?? Enumerable.Empty<ApiResource>())
                    .DistinctBy(x => x.Name),
                configs.SelectMany(x => x.ApiScopes ?? Enumerable.Empty<ApiScope>())
                    .DistinctBy(x => x.Name)));
        }
    }
}