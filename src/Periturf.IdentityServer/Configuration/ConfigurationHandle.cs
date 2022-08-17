using Periturf.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    class ConfigurationHandle : IConfigurationHandle
    {
        private readonly List<Configuration> _configurations;
        private readonly Configuration _config;

        public ConfigurationHandle(List<Configuration> configurations, Configuration config)
        {
            _configurations = configurations;
            _config = config;
        }

        public ValueTask DisposeAsync()
        {
            _configurations.Remove(_config);
            return new ValueTask();
        }
    }
}