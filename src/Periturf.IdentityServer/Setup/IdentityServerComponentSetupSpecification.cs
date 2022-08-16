using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Setup
{
    class IdentityServerComponentSetupSpecification : IWebComponentSetupSpecification
    {
        public IdentityServerComponentSetupSpecification(string name, PathString path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; }

        public PathString Path { get; }

        public ConfigureWebAppDto Configure()
        {
            return new ConfigureWebAppDto(
                new IdentityServerComponent(),
                app => app.Map(Path, idApp => idApp.UseIdentityServer()),
                services => services.AddIdentityServer());
        }
    }
}
