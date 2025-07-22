using System.Text.Json;
using CatFacts.Contracts.Configuration;
using CatFacts.Contracts.DTOs;
using CatFacts.Contracts.Models;
using CatFacts.Contracts.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CatFacts.Tests.Unit;

public class FileFactWriterTests
{
  private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

  [Fact]
  public async Task AppendFact_ValidFact_NotChangedFact()
  {
    var fact = new CatFact(new NonEmptyString("cats have ears"), new PositiveInt(14));
    var guid = Guid.NewGuid();
    var fileOptions = Options.Create<FileStorageOptions>(
      new() { FilePath = $"unittest{guid}.txt", BatchLimit = 5 }
    );

    var service = new FileFactWriter(NullLogger<FileFactWriter>.Instance, fileOptions);
    service.AppendFact(fact);

    await Task.Delay(500); // wait for file operation
    var line = File.ReadLines(fileOptions.Value.FilePath).First();
    var deserializedFact = JsonSerializer.Deserialize<RemoteCatFact>(line, _jsonOptions);
    Assert.NotNull(deserializedFact);
    var readFact = new CatFact(
      new NonEmptyString(deserializedFact.Fact),
      new PositiveInt(deserializedFact.Length)
    );

    Assert.Equal(fact, readFact);
  }
}
