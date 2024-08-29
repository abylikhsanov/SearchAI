using SearchAI.Models;

namespace SearchAI.services;

public class UploadToBlobContainer: IUploadToBlobContainer
{
    internal static DefaultAzureCredential DefaultAzureCredential { get; } = new();
    private readonly BlobContainerClient _blobContainerClient;

    public UploadToBlobContainer(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }
    
    public async Task<UploadDocumentsResponse> UploadFileAsync(IFormFile file)
    {
        try
        {
            List<string> uploadedFiles = [];
            var fileName = file.FileName;
            await using var stream = file.OpenReadStream();
            using var documents = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            for (int i = 0; i < documents.PageCount; i++)
            {
                var documentName = BlobFromFileName(fileName, i);
                var blobClient = _blobContainerClient.GetBlobClient(documentName);
                if (await blobClient.ExistsAsync())
                {
                    continue;
                }

                var tempFileName = Path.GetTempFileName();
                try
                {
                    using var document = new PdfDocument();
                    document.AddPage(documents.Pages[i]);
                    document.Save(tempFileName);
                    await using var tempStream = File.OpenRead(tempFileName);
                    await blobClient.UploadAsync(tempStream, new BlobHttpHeaders { ContentType = "application/pdf" });
                    uploadedFiles.Add(documentName);
                }
                finally
                {
                    File.Delete(tempFileName);
                }
            }

            if (uploadedFiles.Count is 0)
            {
                return UploadDocumentsResponse.FromError("No files uploaded");
            }
            return new UploadDocumentsResponse([.. uploadedFiles]);
        }
        catch (Exception ex)
        {
            return UploadDocumentsResponse.FromError(ex.ToString());
        }
    }

    private static string BlobFromFileName(string fileName, int page = 0) =>
        Path.GetExtension(fileName).ToLower() is ".pdf"
            ? $"{Path.GetFileNameWithoutExtension(fileName)}-{page}.pdf"
            : Path.GetFileName(fileName);
    
}