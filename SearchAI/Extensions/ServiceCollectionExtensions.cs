using OpenAI;

namespace SearchAI.Extensions;


internal static class ServiceCollectionExtensions
{
    private static readonly DefaultAzureCredential s_azureCredential = new();

    internal static IServiceCollection AddAzureServices(this IServiceCollection services)
    {
        services.AddSingleton<BlobServiceClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageAccountEndpoint = config["AzureStorageAccountEndpoint"];
            ArgumentNullException.ThrowIfNull(azureStorageAccountEndpoint);
            var blobServiceClient = new BlobServiceClient(
                new Uri(azureStorageAccountEndpoint), s_azureCredential);
            return blobServiceClient;
        });

        services.AddSingleton<BlobContainerClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureStorageContainer = config["AzureStorageContainer"];
            ArgumentNullException.ThrowIfNull(azureStorageContainer);
            return sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(azureStorageContainer);
        });

        services.AddSingleton<DocumentAnalysisClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureOpenAiServiceEndpoint = config["AzureOpenAiServiceEndpoint"];
            ArgumentNullException.ThrowIfNull(azureOpenAiServiceEndpoint);
            var documentAnalysisClient = new DocumentAnalysisClient(
                new Uri(azureOpenAiServiceEndpoint), s_azureCredential);
            return documentAnalysisClient;
        });

        services.AddSingleton<OpenAIClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var useAOAI = config["UseAOAI"] == "true";
            if (useAOAI)
            {
                var azureOpenAiServiceEndpoint = config["AzureOpenAiServiceEndpoint"];
                ArgumentNullException.ThrowIfNull(azureOpenAiServiceEndpoint);
                var openAIClient = new AzureOpenAIClient(new Uri(azureOpenAiServiceEndpoint), s_azureCredential);
                return openAIClient;
            }
            else
            {
                var openAIAPiKey = config["OpenAIAPIKey"];
                ArgumentNullException.ThrowIfNull(openAIAPiKey);
                var openAIClient = new OpenAIClient(openAIAPiKey);
                return openAIClient;
            }
        });
        
        // Search
        services.AddSingleton<SearchClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureSearchServiceEndpoint = config["AzureSearchServiceEndpoint"];
            ArgumentNullException.ThrowIfNull(azureSearchServiceEndpoint);
            var azureSearchIndexName = config["AzureSearchIndexName"];
            return new SearchClient(new Uri(azureSearchServiceEndpoint), azureSearchIndexName, s_azureCredential);
        });

        services.AddSingleton<SearchIndexClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var azureSearchServiceEndpoint = config["AzureSearchServiceEndpoint"];
            ArgumentNullException.ThrowIfNull(azureSearchServiceEndpoint);
            return new SearchIndexClient(new Uri(azureSearchServiceEndpoint), s_azureCredential);
        });

        return services;
    }
}