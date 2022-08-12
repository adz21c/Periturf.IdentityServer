using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Periturf.IdentityServer.Setup;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Tests.Setup
{
    class IdentityServerComponentTests
    {
        [Test]
        public void Given_Specification_When_Configure_Then_CreateComponent()
        {
            const string name = "Name";
            PathString path = "/Path";

            var sut = new IdentityServerComponentSetupSpecification(name, path);

            var config = sut.Configure();

            Assert.That(sut.Name, Is.EqualTo(name));
            Assert.That(sut.Path, Is.EqualTo(path));
            Assert.That(config.Component, Is.Not.Null);
            Assert.That(config.Component, Is.TypeOf<IdentityServerComponent>());
        }

        [Test]
        public void Given_WebConfigurator_When_IdentityServer_Then_WebComponentSpecAdded()
        {
            var name = A.Dummy<string>();
            var path = A.Dummy<PathString>();

            var configurator = A.Fake<IWebSetupConfigurator>();

            configurator.IdentityServer(name, path);

            A.CallTo(() => configurator.AddWebComponentSpecification(A<IWebComponentSetupSpecification>.That.IsNotNull())).MustHaveHappenedOnceExactly();
        }
    }
}
