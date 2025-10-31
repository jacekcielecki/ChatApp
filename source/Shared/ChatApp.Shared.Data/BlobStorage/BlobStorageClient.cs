using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Shared.Data.BlobStorage;

public class BlobStorageClient : IFileStorage
{
    public async Task<FileUploadResult> Upload(IFormFile file, string filePath)
    {
        var connectionString = Environment.GetEnvironmentVariable(Envars.StorageAccountConnectionString);
        var containerClient = new BlobContainerClient(connectionString, "assets");

        var response = new FileUploadResult();

        try
        {
            BlobClient blobClient = containerClient.GetBlobClient(filePath);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            };

            var tempPath = Path.GetTempFileName();
            await using (var fs = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            await using (var uploadStream = File.OpenRead(tempPath))
            {
                await blobClient.UploadAsync(uploadStream, options);
            }
            File.Delete(tempPath);

            response.IsError = false;
            response.Status = "File uploaded successfully.";
            response.FileName = blobClient.Name;
            response.FileUri = blobClient.Uri.AbsoluteUri;
        }
        catch (RequestFailedException ex)
        {
            response.IsError = true;
            response.Status = ex.Message;
            response.StackTrace = ex.StackTrace;
        }

        return response;
    }
}