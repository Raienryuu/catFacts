using CatFacts.Contracts.Models;
using FluentResults;

namespace CatFacts.Contracts.Services;

public interface ICatFactService
{
  Task<Result<CatFact>> GetCatFact(CancellationToken ct);
}
