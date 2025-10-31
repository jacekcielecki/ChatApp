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

            await using (Stream data = file.OpenReadStream())
            {
                await blobClient.UploadAsync(data, options);
            }

            response.IsError = false;
            response.Status = "File uploaded successfully.";
            response.FileName = blobClient.Name;
            response.FileUri = blobClient.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
            response.IsError = true;
            response.Status = ex.Message;
            response.StackTrace = ex.StackTrace;
        }

        return response;
    }
}