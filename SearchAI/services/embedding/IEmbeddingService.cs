namespace SearchAI.services.embedding;

// Given pdf blob (or text in the future) convert into vector embedding
public interface IEmbeddingService
{
    Task<bool> EmbedPdfBlobAsync(Stream blobStream, string blobName);
    Task CreateSearchIndexAsync(string searchIndexName, CancellationToken ct = default);
    Task EnsureSearchIndexAsync(string searchIndexName, CancellationToken ct = default);
}