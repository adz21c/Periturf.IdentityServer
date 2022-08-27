using Duende.IdentityServer.Models;
using Periturf;

namespace Periturf.IdentityServer.Tests.Integration
{
    public class Tests
    {
        private Environment _environtment;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _environtment = Periturf.Environment.Setup(s =>
            {
                s.GenericHost(h =>
                {
                    h.Web(wh => 
                    {
                        wh.BindToUrl("http://localhost:36251");
                        wh.IdentityServer("IdentityServer", "/IdentityServer");
                    });
                });
            });
            await _environtment.StartAsync();
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await _environtment.StopAsync();
        }


        [Test]
        public async Task Test1Async()
        {
            await using var handle = await _environtment.ConfigureAsync(c => c.IdentityServer("IdentityServer", c =>
            {
                c.Client(new Client
                {
                    ClientId = "Client1"
                });
            }));
            

        }
    }
}