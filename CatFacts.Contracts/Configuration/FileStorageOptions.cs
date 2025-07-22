namespace CatFacts.Contracts.Configuration;

public class FileStorageOptions
{
  public static string Key { get; set; } = "FileStorage";
  public required string FilePath { get; set; }
  public required int BatchLimit { get; set; }
}
