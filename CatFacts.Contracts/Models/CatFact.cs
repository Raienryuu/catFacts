namespace CatFacts.Contracts.Models;

public record CatFact(NonEmptyString Fact, PositiveInt Length);

public record NonEmptyString
{
  public string Value { get; private set; } = null!;

  public NonEmptyString(string value)
  {
    if (string.IsNullOrEmpty(value))
      throw new InvalidOperationException($"{nameof(value)} should be non empty string.");
    Value = value;
  }
}

public record PositiveInt
{
  public int Value { get; private set; }

  public PositiveInt(int value)
  {
    if (value < 1)
      throw new InvalidOperationException($"{nameof(value)} should be positive integer.");
    Value = value;
  }
}
