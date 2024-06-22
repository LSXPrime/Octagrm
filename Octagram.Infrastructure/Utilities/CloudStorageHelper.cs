using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Octagram.Application.Interfaces;

namespace Octagram.Infrastructure.Utilities;

public class CloudStorageHelper(IConfiguration configuration) : ICloudStorageHelper
{
    private readonly string? _connectionString = configuration.GetConnectionString("AzureStorage");
    private readonly string? _containerName = configuration["AzureStorage:ContainerName"];

    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="fileStream">The stream containing the file data.</param>
    /// <param name="fileName">The original name of the file.</param>
    /// <param name="folderName">The folder name within the Azure Blob Storage container where the file should be stored.</param>
    /// <returns>
    /// The URI of the uploaded file in Azure Blob Storage.
    /// </returns>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobName = $"{folderName}/{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(fileStream, true);

        return blobClient.Uri.ToString();
        
        /*
        // local storage, just for testing purposes
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream);
        return filePath;
        */
    }
}