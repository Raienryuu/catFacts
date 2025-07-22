using CatFacts.Contracts.Models;

namespace CatFacts.Contracts.Services;

public interface IFactWriter
{
  void AppendFact(CatFact catFact);
}
