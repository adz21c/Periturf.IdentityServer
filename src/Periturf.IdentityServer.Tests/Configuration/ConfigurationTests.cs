using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Periturf.Configuration;
using Periturf.IdentityServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Tests.Configuration
{
    class ConfigurationTests
    {
        [Test]
        public void Given_ConfigurationContext_When_IdentityServer_Then_NewConfigSpecificationAdded()
        {
            var name = A.Dummy<string>();
            var _sut = new IdentityServerConfigurationSpecification();

            var configContext = A.Fake<IConfigurationContext>();
            A.CallTo(() => configContext.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(A<string>._)).Returns(_sut);

            configContext.IdentityServer(name);

            A.CallTo(() => configContext.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(name)).MustHaveHappened().Then(
                A.CallTo(() => configContext.AddSpecification(_sut)).MustHaveHappenedOnceExactly());
        }
    }
}
