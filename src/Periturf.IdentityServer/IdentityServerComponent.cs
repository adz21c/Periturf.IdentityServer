using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.Verify;

namespace Periturf.IdentityServer
{
    class IdentityServerComponent : IComponent
    {
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
            throw new System.NotImplementedException();
        }
    }
}
