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


        [Test]
        public async Task Given_Clients_When_ConfigureClient_Then_ClientConfigOverriden()
        {
            const string IdClientSecret = "Secret";
            const string IdClientSecret2 = "Secret2";
            const string IdClientId = "Client2";
            var idScope = new ApiScope
            {
                Name = "Scope",
                Enabled = true
            };
            await using var initialConfigHandle = await _environtment.ConfigureAsync(c => c.IdentityServer(c =>
            {
                c.Client(new Client
                {
                    ClientId = IdClientId,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> { new Secret(IdClientSecret.Sha256()) },
                    AccessTokenType = AccessTokenType.Jwt,
                    Enabled = true,
                    AllowedScopes = new List<string> { idScope.Name }
                });
                c.Scope(idScope);
            }));

            using var client = new HttpClient();

            var assumptionTokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = IdClientId,
                ClientSecret = IdClientSecret,
                Scope = idScope.Name
            });
            Assume.That(assumptionTokenResponse, Is.Not.Null);
            Assume.That(assumptionTokenResponse.IsError, Is.False);
            Assume.That(assumptionTokenResponse.AccessToken, Is.Not.Null);

            await using var configHandle = await _environtment.ConfigureAsync(c => c.IdentityServer(c =>
            {
                c.Client(new Client
                {
                    ClientId = IdClientId,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> { new Secret(IdClientSecret2.Sha256()) },
                    AccessTokenType = AccessTokenType.Jwt,
                    Enabled = true,
                    AllowedScopes = new List<string> { idScope.Name }
                });
            }));

            var oldSecretResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = IdClientId,
                ClientSecret = IdClientSecret,
                Scope = idScope.Name
            });
            var newSecretResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = IdClientId,
                ClientSecret = IdClientSecret2,
                Scope = idScope.Name
            });

            Assert.That(oldSecretResponse, Is.Not.Null);
            Assert.That(oldSecretResponse.IsError, Is.True);
            Assert.That(oldSecretResponse.Error, Is.EqualTo("invalid_client"));

            Assert.That(newSecretResponse, Is.Not.Null);
            Assert.That(newSecretResponse.IsError, Is.False);
            Assert.That(newSecretResponse.AccessToken, Is.Not.Null);
            Assert.That(newSecretResponse.TokenType, Is.EqualTo("Bearer"));
        }
    }
}