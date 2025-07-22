namespace CatFacts.Contracts.DTOs;

public class RemoteCatFact
{
  public string Fact { get; set; } = null!;
  public int Length { get; set; }

  public static bool IsResponseValid(RemoteCatFact? fact)
  {
    if (fact is null)
      return false;
    if (fact.Fact is null || fact.Fact.Length == 0)
      return false;
    if (fact.Length < 0)
      return false;
    return true;
  }
}
