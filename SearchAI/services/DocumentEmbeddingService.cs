using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
namespace SearchAI.services;

public class DocumentEmbeddingService : IDocumentEmbeddingService
{
    private readonly IConfiguration _configuration;
    private readonly SearchClient _searchClient;

    public DocumentEmbeddingService(IConfiguration configuration, SearchClient searchClient)
    {
        _configuration = configuration;
        _searchClient = searchClient;
    }

    private string GenerateVectorEmbedding(string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }
    private List<string> ChunkText(string text, int chunkSize)
    {
        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += chunkSize)
        {
            chunks.Add(text.Substring(i, Math.Min(chunkSize, text.Length - i)));
        }

        return chunks;
    }

    public async Task StoreVectorEmbeddingAsync(string documentId, string documentText)
    {
        var chunks = ChunkText(documentText, 1000);
        var documents = chunks.Select((chunk, index) => new
        {
            Id = $"{documentId}{index}",
            Content = chunk,
            VectorEmbedding = GenerateVectorEmbedding(chunk)
        }).ToList();
        
        var batch = IndexDocumentsBatch.Upload(documents);
        var options = new IndexDocumentsOptions { ThrowOnAnyError = true };
        await _searchClient.IndexDocumentsAsync(batch, options);
    }
}