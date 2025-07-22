using CatFacts.Api.Endpoints.CatFact;

namespace CatFacts.Api.Endpoints;

public static class CatFactsMapper
{
  public static WebApplication MapCatFactsEndpoints(this WebApplication app)
  {
    app.MapGetCatFactEndpoint();

    return app;
  }
}
