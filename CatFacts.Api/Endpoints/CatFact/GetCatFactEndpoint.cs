using CatFacts.Api.Endpoints.Responses;
using CatFacts.Contracts.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CatFacts.Api.Endpoints.CatFact;

public static class GetCatFactEndpoint
{
  public static WebApplication MapGetCatFactEndpoint(this WebApplication app)
  {
    app.MapGet(
        EndpointRoutes.CatFacts.GET_CAT_FACT,
        async Task<Results<Ok<GetCatFactResponse>, ProblemHttpResult>> (
          ICatFactService catFactService,
          IFactWriter factWriter,
          CancellationToken ct
        ) =>
        {
          var fact = await catFactService.GetCatFact(ct);
          if (fact.IsFailed)
          {
            return TypedResults.Problem(
              fact.Errors[0].Message,
              statusCode: (int)fact.Errors[0].Metadata.GetValueOrDefault("StatusCode")!
            );
          }

          factWriter.AppendFact(fact.Value);

          return TypedResults.Ok(fact.Value.MapToGetCatFactResponse());
        }
      )
      .WithName(nameof(GetCatFactEndpoint));

    return app;
  }
}
