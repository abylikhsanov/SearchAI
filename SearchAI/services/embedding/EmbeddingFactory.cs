using SearchAI.Models;

namespace SearchAI.services.embedding;

public sealed class EmbeddingFactory(IEnumerable<IEmbeddingService> embeddingServices)
{
    public IEmbeddingService GetEmbeddingService(EmbeddingType embeddingType)
    {
        return embeddingType switch
        {
            EmbeddingType.AzureSearch => embeddingServices.OfType<AzureSearchEmbedding>().Single(),
            _ => throw new ArgumentException("Unsupported embedding type", nameof(embeddingType))
        };
    }
}