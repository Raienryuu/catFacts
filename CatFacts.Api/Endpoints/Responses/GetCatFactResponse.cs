namespace CatFacts.Api.Endpoints.Responses;

public class GetCatFactResponse
{
  public string Fact { get; set; } = null!;
  public int Length { get; set; }
}
