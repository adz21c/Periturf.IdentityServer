using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    [ExcludeFromCodeCoverage]
    class ClientStore : IClientStore
    {
        private readonly ConfigurationStore _store;

        public ClientStore(ConfigurationStore store)
        {
            _store = store;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return _store.FindClientByIdAsync(clientId);
        }
    }
}