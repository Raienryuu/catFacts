using CatFacts.Contracts.Configuration;
using CatFacts.Contracts.DTOs;
using CatFacts.Contracts.Models;
using CatFacts.Contracts.Results;
using FluentResults;
using Microsoft.Extensions.Options;

namespace CatFacts.Contracts.Services;

public class CatFactService(
  IOptions<CatFactSourceOptions> catFactSourceOptions,
  ILogger<CatFactService> logger,
  HttpClient httpClient
) : ICatFactService
{
  public async Task<Result<CatFact>> GetCatFact(CancellationToken ct)
  {
    try
    {
      var response = await httpClient.GetAsync(catFactSourceOptions.Value.Url, ct);
      if (!response.IsSuccessStatusCode)
        return CouldNotRetrieveCatFact();

      var catFact = await response.Content.ReadFromJsonAsync<RemoteCatFact>(cancellationToken: ct);

      var isValid = RemoteCatFact.IsResponseValid(catFact);
      if (!isValid)
        return CouldNotRetrieveCatFact();

      return Result.Ok(new CatFact(new NonEmptyString(catFact!.Fact), new PositiveInt(catFact.Length)));
    }
    catch (HttpRequestException e)
    {
      logger.LogError("There was an exception while trying to get cat fact. {ex}", e);
    }
    return CouldNotRetrieveCatFact();
  }

  private Result<CatFact> CouldNotRetrieveCatFact()
  {
    logger.LogError("Could not retrieve a cat fact.");
    return Result.Fail(new ErrorWithStatusCode("Could not retrieve a cat fact. Try again later.", 404));
  }
}
