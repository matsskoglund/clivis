using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Clivis;

namespace IntTestClivis
{

    public class Tests
    {

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public Tests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            _client = _server.CreateClient();

        }

        [Fact]
        public async Task ReturnFoundItem()
        {
            // Act
            var response = await _client.GetAsync("/api/climate");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("[{\"key\":\"Nyckel\",\"name\":\"Item1\",\"isComplete\":false}]",
            responseString);
        }

    
    }
}
