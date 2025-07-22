namespace CatFacts.Contracts.Configuration;

public class CatFactSourceOptions
{
  public static string Key { get; set; } = "CatFactSource";
  public required string Url { get; set; }
}
