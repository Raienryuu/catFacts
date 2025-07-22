using CatFacts.Contracts.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;

namespace CatFacts.Tests.E2E;

public class GetCatFactEndpointTests(WebApplicationFactory<Program> webApplicationFactory)
  : IClassFixture<WebApplicationFactory<Program>>
{
  [Fact]
  public async Task GetCatFact()
  {
    var client = webApplicationFactory.CreateClient();
    var options = webApplicationFactory.Services.GetRequiredService<IOptions<FileStorageOptions>>().Value;

    var response = await client.GetAsync("api/v1/catfacts");

    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    Assert.Contains("cat", content);
    await Task.Delay(500);

    var line = File.ReadLines(options.FilePath).First();
    File.Delete(options.FilePath); // delete first or assert will stop the execution
    Assert.Equal(line, content);
  }
}
