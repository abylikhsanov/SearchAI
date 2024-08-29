using Azure;
using Azure.Search.Documents.Indexes.Models;
using OpenAI;
using SearchAI.Models;

namespace SearchAI.services.embedding;

public class AzureSearchEmbedding(
    OpenAIClient openAiClient,
    string embeddingModelName,
    SearchClient searchClient,
    string searchIndexName,
    SearchIndexClient searchIndexClient,
    DocumentAnalysisClient documentAnalysisClient,
    BlobContainerClient corpusContainerClient,
    ILogger<AzureSearchEmbedding>? logger = null
    ) : IEmbeddingService
{
    public async Task<bool> EmbedPdfBlobAsync(Stream blobStream, string blobName)
    {
        return true;
    }

    public async Task CreateSearchIndexAsync(string searchIndexName, CancellationToken ct = default)
    {
        var vectorSearchConfigName = "vector-config-name";
        var vectorSearchProfile = "vector-profile";
        var index = new SearchIndex(searchIndexName)
        {
            VectorSearch = new()
            {
                Algorithms = { new HnswAlgorithmConfiguration(vectorSearchConfigName) },
                Profiles = { new VectorSearchProfile(vectorSearchProfile, vectorSearchConfigName) }
            },
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = false },
                new SearchableField("content") { AnalyzerName = LexicalAnalyzerName.EnMicrosoft },
                new SimpleField("category", SearchFieldDataType.String) { IsFacetable = true },
                new SimpleField("sourcepage", SearchFieldDataType.String) { IsFacetable = true },
                new SimpleField("sourcefile", SearchFieldDataType.String) { IsFacetable = false },
                new SearchField("embedding", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    VectorSearchDimensions = 1536,
                    IsSearchable = true,
                    VectorSearchProfileName = vectorSearchProfile
                }
            },
            SemanticSearch = new()
            {
                Configurations =
                {
                    new SemanticConfiguration("default", new()
                    {
                        ContentFields =
                        {
                            new SemanticField("content")
                        }
                    })
                }
            }

        };
        await searchIndexClient.CreateIndexAsync(index, ct);
        logger?.LogWarning("Created search index with name {}", searchIndexName);
    }
    
    public async Task EnsureSearchIndexAsync(string searchIndexName, CancellationToken ct = default)
    {
        var indexNames = searchIndexClient.GetIndexNamesAsync();
        await foreach (var page in indexNames.AsPages())
        {
            if (page.Values.Any(indexName => indexName == searchIndexName))
            {
                logger?.LogWarning("Search index '{indexName}' already exists", searchIndexName);
                return;
            }
        }

        await CreateSearchIndexAsync(searchIndexName, ct);
    }
    
    public async Task<IReadOnlyList<PageDetail>> GetDocumentTextAsync(Stream blobStream, string blobName)
    {
        logger?.LogWarning("Extacting text from blob name {} using Azure Document Analyzer", blobName);
        using var ms = new MemoryStream();
        blobStream.CopyTo(ms);
        ms.Position = 0;
        AnalyzeDocumentOperation operation =
            documentAnalysisClient.AnalyzeDocument(WaitUntil.Started, "prebuilt-layout", ms);

        var offset = 0;
        List<PageDetail> pageMap = [];
        var result = await operation.WaitForCompletionAsync();
        
    }
}