namespace SearchAI.services.embedding;

public sealed class EmbeddingFactory(IEnumerable<IEmbeddingService> embeddingServices)
{
    public IEmbeddingService GetEmbeddingService(string embeddingName)
}