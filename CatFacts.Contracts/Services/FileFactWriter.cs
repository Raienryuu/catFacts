using System.Collections.Concurrent;
using System.Text.Json;
using CatFacts.Contracts.Configuration;
using CatFacts.Contracts.Models;
using Microsoft.Extensions.Options;

namespace CatFacts.Contracts.Services;

public class FileFactWriter(ILogger<FileFactWriter> logger, IOptions<FileStorageOptions> fileStorageOptions)
  : IFactWriter
{
  private readonly string _filePath = fileStorageOptions.Value.FilePath;
  private readonly ConcurrentQueue<string> _factsToWrite = new();
  private bool _isWriting = false;
  private readonly Lock _writingLock = new();
  private readonly int _batchLimit = fileStorageOptions.Value.BatchLimit;

  public void AppendFact(CatFact catFact)
  {
    ArgumentNullException.ThrowIfNull(catFact);
    ArgumentNullException.ThrowIfNull(catFact.Fact);
    ArgumentNullException.ThrowIfNull(catFact.Length);

    var factAsString = JsonSerializer.Serialize(
      new { fact = catFact.Fact.Value, length = catFact.Length.Value }
    );

    _factsToWrite.Enqueue(factAsString);
    lock (_writingLock)
    {
      if (_isWriting == false)
      {
        _isWriting = true;
        Task.Run(WriteToFile);
      }
      ;
    }
  }

  private void WriteToFile()
  {
    var batch = new List<string>();
    try
    {
      while (_factsToWrite.TryDequeue(out var fact) && batch.Count < _batchLimit)
      {
        batch.Add(fact);
      }
      File.AppendAllLines(_filePath, batch);
    }
    catch (FileNotFoundException e)
    {
      logger.LogCritical(
        "{fileWriter} did not find file at the path, {exception}",
        nameof(FileFactWriter),
        e
      );
    }
    lock (_writingLock)
    {
      _isWriting = false;
    }
  }
}
