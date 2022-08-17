using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    class ClientStore : IClientStore
    {
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }
    }
}