using CatFacts.Api;
using CatFacts.Api.Endpoints;
using CatFacts.Contracts.Configuration;
using CatFacts.Contracts.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<ICatFactService, CatFactService>();

builder.Services.AddSingleton<IFactWriter, FileFactWriter>();

builder.Services.Configure<CatFactSourceOptions>(
  builder.Configuration.GetRequiredSection(CatFactSourceOptions.Key)
);

builder.Services.Configure<FileStorageOptions>(
  builder.Configuration.GetRequiredSection(FileStorageOptions.Key)
);

builder.Services.AddExceptionHandler<UncaughtExceptionHandler>();
builder.Services.AddHttpClient<ICatFactService, CatFactService>();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapCatFactsEndpoints();

Console.WriteLine("Visit http://localhost:5212/scalar for generated openAPI request panel");

app.Run();
