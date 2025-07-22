using CatFacts.Api.Endpoints.Responses;

namespace CatFacts.Api.Endpoints;

public static class MappingExtensions
{
  public static GetCatFactResponse MapToGetCatFactResponse(this Contracts.Models.CatFact fact)
  {
    return new() { Length = fact.Length.Value, Fact = fact.Fact.Value };
  }
}
