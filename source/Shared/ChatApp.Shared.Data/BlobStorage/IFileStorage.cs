using Microsoft.AspNetCore.Http;

namespace ChatApp.Shared.Data.BlobStorage;

public interface IFileStorage
{
    Task<FileUploadResult> Upload(IFormFile file, string filePath);
}
