using Duende.IdentityServer.Models;
using IdentityModel.Client;
using Periturf;
using System.Net;

namespace Periturf.IdentityServer.Tests.Integration
{
    public class ClientOverrideTests
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
        public async Task Given_NoClients_When_Auth_Then_ClientError()
        {
            var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
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
                ClientId = "UnknownClient",
                ClientSecret = "UnknownSecret",
                Scope = "UnknownScope"
            });

            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse.IsError, Is.True);
        }




        [Test]
        public async Task Given_NoClients_When_ConfigureClient_Then_AuthWorks()
        {
            const string IdClientSecret = "Secret";
            var idScope = new ApiScope
            {
                Name = "Scope",
                Enabled = true
            };
            var idClient = new Client
            {
                ClientId = "Client1",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> { new Secret(IdClientSecret.Sha256()) },
                AccessTokenType = AccessTokenType.Jwt,
                Enabled = true,
                AllowedScopes = new List<string> { idScope.Name }
            };
            await using var configHandle = await _environtment.ConfigureAsync(c => c.IdentityServer(c =>
            {
                c.Client(idClient);
                c.Scope(idScope);
            }));

            var client = new HttpClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
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
                Scope = idScope.Name
            });

            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse.IsError, Is.False);
            Assert.That(tokenResponse.AccessToken, Is.Not.Null);
            Assert.That(tokenResponse.TokenType, Is.EqualTo("Bearer"));
        }
    }
}