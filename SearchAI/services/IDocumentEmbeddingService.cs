namespace SearchAI.services;

public interface IDocumentEmbeddingService
{
    Task StoreVectorEmbeddingAsync(string documentId, string documentText);
}