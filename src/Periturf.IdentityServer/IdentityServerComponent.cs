using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.IdentityServer.Configuration;
using Periturf.Verify;

namespace Periturf.IdentityServer
{
    class IdentityServerComponent : IComponent
    {
        private readonly ConfigurationStore _configStore;

        public IdentityServerComponent(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        public IComponentClient CreateClient()
        {
            throw new System.NotImplementedException();
        }

        public IConditionBuilder CreateConditionBuilder()
        {
            throw new System.NotImplementedException();
        }

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new IdentityServerConfigurationSpecification(_configStore);
        }
    }
}
