using Duende.IdentityServer.Configuration;
using System;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdentityServerSetupConfigurator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        void Options(Action<IdentityServerOptions> options);
    }
}