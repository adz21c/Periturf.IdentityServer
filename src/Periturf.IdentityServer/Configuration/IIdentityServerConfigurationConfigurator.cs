using Duende.IdentityServer.Models;

namespace Periturf.IdentityServer.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdentityServerConfigurationConfigurator
    {
        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.Client"/>
        /// </summary>
        /// <param name="client"></param>
        void Client(Client client);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.ApiResource"/>
        /// </summary>
        /// <param name="resource"></param>
        void ApiResource(ApiResource resource);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.IdentityResource"/>
        /// </summary>
        /// <param name="resource"></param>
        void IdentityResource(IdentityResource resource);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.ApiScope"/>
        /// </summary>
        /// <param name="scope"></param>
        void Scope(ApiScope scope);
    }
}