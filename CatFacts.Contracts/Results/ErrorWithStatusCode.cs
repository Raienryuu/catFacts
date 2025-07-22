using FluentResults;

namespace CatFacts.Contracts.Results;

public class ErrorWithStatusCode(string errorMessage, int statusCode) : IError
{
  public List<IError> Reasons => throw new NotImplementedException();

  public string Message => errorMessage;

  public Dictionary<string, object> Metadata => new() { { "StatusCode", statusCode } };
}
