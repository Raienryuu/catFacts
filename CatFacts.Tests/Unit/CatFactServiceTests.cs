using System.Net;
using System.Text;
using System.Text.Json;
using CatFacts.Contracts.Configuration;
using CatFacts.Contracts.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CatFacts.Tests.Unit;

public class CatFactServiceTests
{
  public class ValidStubHandler : HttpMessageHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken
    )
    {
      return Task.FromResult(
        new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.OK,
          Content = new StringContent(
            JsonSerializer.Serialize(new { fact = "cats have 4 legs", length = 16 }),
            Encoding.UTF8,
            "application/json"
          ),
        }
      );
    }
  }

  public class ReturnsNullHandler : HttpMessageHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken
    )
    {
      return Task.FromResult(
        new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, Content = null }
      );
    }
  }

  public class MalformedHandler : HttpMessageHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken
    )
    {
      return Task.FromResult(
        new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.NotFound,
          Content = new StringContent("{\"327}", Encoding.UTF8, "application/json"),
        }
      );
    }
  }

  public class NegativeLengthHandler : HttpMessageHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken
    )
    {
      return Task.FromResult(
        new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.NotFound,
          Content = new StringContent(
            JsonSerializer.Serialize(new { fact = "cats have 4 legs", length = -16 }),
            Encoding.UTF8,
            "application/json"
          ),
        }
      );
    }
  }

  private static CatFactService GetServiceInstance(HttpMessageHandler handler)
  {
    var sourceOptions = new CatFactSourceOptions() { Url = "http://testing" };
    var options = Options.Create(sourceOptions);
    var httpClient = new HttpClient(handler);
    return new CatFactService(options, NullLogger<CatFactService>.Instance, httpClient);
  }

  [Fact]
  public async Task GetCatFact_ValidResponse_Success()
  {
    var factService = GetServiceInstance(new ValidStubHandler());

    var result = await factService.GetCatFact(CancellationToken.None);

    Assert.NotNull(result);
    Assert.True(result.IsSuccess);
    Assert.Equal(result.Value.Fact.Value.Length, result.Value.Length.Value);
  }

  [Fact]
  public async Task GetCatFact_NullResponse_404NotFound()
  {
    var factService = GetServiceInstance(new ReturnsNullHandler());

    var result = await factService.GetCatFact(CancellationToken.None);

    Assert.NotNull(result);
    Assert.True(result.IsFailed);
    Assert.Equal(
      (int)HttpStatusCode.NotFound,
      (int)result.Errors[0].Metadata.GetValueOrDefault("StatusCode")!
    );
  }

  [Fact]
  public async Task GetCatFact_MalformedResponse_404NotFound()
  {
    var factService = GetServiceInstance(new MalformedHandler());

    var result = await factService.GetCatFact(CancellationToken.None);

    Assert.NotNull(result);
    Assert.True(result.IsFailed);
    Assert.Equal(
      (int)HttpStatusCode.NotFound,
      (int)result.Errors[0].Metadata.GetValueOrDefault("StatusCode")!
    );
  }

  [Fact]
  public async Task GetCatFact_NegativeLength_404NotFound()
  {
    var factService = GetServiceInstance(new NegativeLengthHandler());

    var result = await factService.GetCatFact(CancellationToken.None);

    Assert.NotNull(result);
    Assert.True(result.IsFailed);
    Assert.Equal(
      (int)HttpStatusCode.NotFound,
      (int)result.Errors[0].Metadata.GetValueOrDefault("StatusCode")!
    );
  }
}
