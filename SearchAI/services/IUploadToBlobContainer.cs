using SearchAI.Models;

namespace SearchAI.services;

public interface IUploadToBlobContainer
{
    Task<UploadDocumentsResponse> UploadFileAsync(IFormFile file);
}