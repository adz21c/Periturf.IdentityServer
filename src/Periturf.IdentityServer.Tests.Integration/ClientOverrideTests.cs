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
        private DiscoveryDocumentResponse _discoveryDocument;

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

            using var client = new HttpClient();
            _discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = IdentityServerUrl,
                Policy = new DiscoveryPolicy
                {
                    ValidateIssuerName = false,
                    RequireHttps = false
                }
            });

            Assume.That(_discoveryDocument, Is.Not.Null.And.Property("IsError").False);
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

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = "UnknownClient",
                ClientSecret = "UnknownSecret",
                Scope = "UnknownScope"
            });

            Assert.That(tokenResponse, Is.Not.Null);
            Assert.That(tokenResponse.IsError, Is.True);
            Assert.That(tokenResponse.Error, Is.EqualTo("invalid_client"));
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

            using var client = new HttpClient();

            var assumptionTokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = idClient.ClientId,
                ClientSecret = IdClientSecret,
                Scope = idScope.Name
            });
            Assume.That(assumptionTokenResponse, Is.Not.Null);
            Assume.That(assumptionTokenResponse.IsError, Is.True);
            Assume.That(assumptionTokenResponse.Error, Is.EqualTo("invalid_client"));

            await using var configHandle = await _environtment.ConfigureAsync(c => c.IdentityServer(c =>
            {
                c.Client(idClient);
                c.Scope(idScope);
            }));

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
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