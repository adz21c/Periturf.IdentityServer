//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

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
