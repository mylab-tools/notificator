using System.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Http;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class GoogleOAuth2
    {
        private readonly ITestOutputHelper _output;
        private readonly GoogleCredential _credentialsData;

        public GoogleOAuth2(ITestOutputHelper output)
        {
            _output = output;
            _credentialsData = GoogleCredential.FromFile("credentials.json")
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                .CreateWithHttpClientFactory(new TestHttpClientFactory());
        }

        [Fact]
        public async Task ShouldRequestAccessToken()
        {
            //Arrange

            //Act
            
            var accessToken = await _credentialsData.UnderlyingCredential.GetAccessTokenForRequestAsync();

            _output.WriteLine(accessToken);

            //Assert
            Assert.NotNull(accessToken);
        }

        [Fact]
        public async Task ShouldCacheRequestedAccessToken()
        {
            //Arrange

            //Act

            var accessToken1 = await _credentialsData.UnderlyingCredential.GetAccessTokenForRequestAsync();
            var accessToken2 = await _credentialsData.UnderlyingCredential.GetAccessTokenForRequestAsync();

            _output.WriteLine("Access token 1: " + accessToken1);
            _output.WriteLine("Access token 2: " + accessToken2);

            //Assert
            Assert.Equal(accessToken1, accessToken2);
        }

        class TestHttpClientFactory : Google.Apis.Http.IHttpClientFactory
        {
            public ConfigurableHttpClient CreateHttpClient(CreateHttpClientArgs args)
            {
                var httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                };
                var handler = new ConfigurableMessageHandler(httpMessageHandler);
                return new ConfigurableHttpClient(handler);
            }
        }
    }
    
}