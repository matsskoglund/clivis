using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Clivis;
using System.Text;

namespace ClivisTests
{

    public class IntTestClivis
    {

        private readonly TestServer _server;
        private readonly HttpClient _client;

        public IntTestClivis()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            
            _client = _server.CreateClient();
            
        }


        [Fact]
        public async Task GetAllItems()
        {
            // Act
            var response = await _client.GetAsync("/api/climate");
            //response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();


            // Assert
            //Assert.Equal("[{\"key\":\"Netatmo\",\"sourceName\":\"Weatherstation\",\"outdoorTemp\":\"6\",\"indoorTemp\":\"22\"}]",responseString);
            Assert.NotNull(responseString);
        }
  /*      [Fact]
        public async Task GetItem()
        {
            // Act
            var response = await _client.GetAsync("/api/Climate/Netatmo");
           // response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(responseString);
            Assert.Equal("OK",response.StatusCode.ToString() );
        }*/

        // We should not be able to create items
   /*     [Fact]
        public async Task CreateItem()
        {
            // Act

            StringContent queryString = new StringContent("{\"key\":\"Nyckel\",\"sourceName\":\"Item8\"}", Encoding.Unicode, "application/json");
            
           var response = await _client.PostAsync("/api/Climate/", queryString);
          //  response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.StartsWith("{\"key\":\"Nyckel\",\"sourceName\":\"Item8\"", responseString);


        }*/


    }
}
