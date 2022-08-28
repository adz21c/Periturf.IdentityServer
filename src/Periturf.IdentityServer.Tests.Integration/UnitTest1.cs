using Duende.IdentityServer.Models;
using IdentityModel.Client;
using Periturf;
using System.Net;

namespace Periturf.IdentityServer.Tests.Integration
{
    public class Tests
    {
        private const string HostUrl = "http://localhost:36251";
        private const string IdentityServerUrl = HostUrl + "/IdentityServer";
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
                        wh.BindToUrl(HostUrl);
                        wh.WebApp();
                        wh.IdentityServer();
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
            const string IdClientSecret = "Secret";
            var idClient = new Client
            {
                ClientId = "Client1",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> { new Secret(IdClientSecret.Sha256()) }
            };
            await _environtment.ConfigureAsync(c => c.IdentityServer(c =>
            {
                c.Client(idClient);
            }));

            var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
                Address = IdentityServerUrl,
                Policy = new DiscoveryPolicy
                {
                    ValidateIssuerName = false,
                    RequireHttps = false
                }
            });
            if (discoveryDocument.IsError)
                Assert.Fail("Discovery failed");

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = idClient.ClientId,
                ClientSecret = IdClientSecret,
                
            });

            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse.IsError, Is.False);
            Assert.That(tokenResponse.AccessToken, Is.Not.Null);
            Assert.That(tokenResponse.IdentityToken, Is.Not.Null);
            Assert.That(tokenResponse.TokenType, Is.EqualTo("Bearer"));
        }
    }
}