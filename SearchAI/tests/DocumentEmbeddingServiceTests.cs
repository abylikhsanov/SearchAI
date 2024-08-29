using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using SearchAI.services;
using Xunit;
namespace SearchAI.tests;


public class DocumentEmbeddingServiceTests
{
    private readonly Mock<SearchClient> _mockSearchClient;
    private readonly IConfiguration _configuration;

    public DocumentEmbeddingServiceTests()
    {
        _mockSearchClient = new Mock<SearchClient>();
        var inMemorySettings = new Dictionary<string, string>
        {
            { "AzureSearch:SearchServiceEndpoint", "https://fake.endpoint" },
            { "AzureSearch:ApiKey", "fake_api_key" },
            { "AzureSearch:IndexName", "fake_index" },
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task StoreVectorEmbeddingsAsync_ShouldIndexDocuments()
    {
        var service = new DocumentEmbeddingService(_configuration, _mockSearchClient.Object);
        var documentId = "test_document";
        var documentText = "This is a sample text to test the embedding";

        var mockResponse = new Mock<Response<IndexDocumentsResult>>();
    }
}