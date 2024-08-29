using Azure;
using Azure.Search.Documents;
using SearchAI.Extensions;
using SearchAI.services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
var searchEnpoint = new Uri(configuration["AzureSearch:SearchServiceEndpoint"]);
var indexName = configuration["AzureSearch:IndexName"];
var apiKey = new AzureKeyCredential(configuration["AzureSearch:ApiKey"]);
builder.Services.AddSingleton(new SearchClient(searchEnpoint, indexName, apiKey));
builder.Services.AddSingleton<IDocumentEmbeddingService, DocumentEmbeddingService>();

builder.Services.AddAzureServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
