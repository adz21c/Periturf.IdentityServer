using Microsoft.AspNetCore.Http;
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
            throw new NotImplementedException();
        }
    }
}
