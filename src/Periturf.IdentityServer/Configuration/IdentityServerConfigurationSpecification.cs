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
        public Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
