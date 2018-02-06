using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConfigurationService.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace ConfigurationService.Integration.Tests
{
    public class DevicesEndpointTests
    {
        private readonly HttpClient _client;

        public DevicesEndpointTests()
        {
            var server = new TestServer(ConfigurationService.Program.CreateBaseHostBuilder(new string[] { })
                .ConfigureAppConfiguration((ctx, builder) =>
                    builder.AddJsonFile("appsettings.Tests.json")));
            _client = server.CreateClient();
        }

        [Fact]
        public async Task ShouldReturnEmptyCollectionOfDevices()
        {
            var response = await _client.GetAsync("/api/devices");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var devices = JsonConvert.DeserializeObject<List<Device>>(responseContent);
            Assert.Empty(devices);
        }
    }
}
