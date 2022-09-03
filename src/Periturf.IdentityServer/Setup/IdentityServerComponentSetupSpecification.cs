using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Periturf.IdentityServer.Configuration;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Setup
{
    class IdentityServerComponentSetupSpecification : IWebComponentSetupSpecification, IIdentityServerSetupConfigurator
    {
        private Action<IdentityServerOptions>? _options;

        public IdentityServerComponentSetupSpecification(string name, PathString path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; }

        public PathString Path { get; }

        public void Options(Action<IdentityServerOptions> options)
        {
            _options = options;
        }

        public ConfigureWebAppDto Configure()
        {
            var configurationStore = new ConfigurationStore();
            return new ConfigureWebAppDto(
                new IdentityServerComponent(configurationStore),
                app => app.UseIdentityServer(),
                services =>
                {
                    services.AddSingleton(configurationStore);
                    services.AddIdentityServer(_options ?? (o => { }))
                        .AddClientStore<ClientStore>()
                        .AddResourceStore<ResourceStore>();
                });
        }
    }
}
