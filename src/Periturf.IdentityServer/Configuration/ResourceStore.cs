using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    [ExcludeFromCodeCoverage]
    class ResourceStore : IResourceStore
    {
        private readonly ConfigurationStore _store;

        public ResourceStore(ConfigurationStore store)
        {
            _store = store;
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return _store.FindApiResourcesByNameAsync(apiResourceNames);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return _store.FindApiResourcesByScopeNameAsync(scopeNames);
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            return _store.FindApiScopesByNameAsync(scopeNames);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return _store.FindIdentityResourcesByScopeNameAsync(scopeNames);
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return _store.GetAllResourcesAsync();
        }
    }
}