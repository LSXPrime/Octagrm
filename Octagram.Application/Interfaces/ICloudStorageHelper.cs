namespace Octagram.Application.Interfaces;

public interface ICloudStorageHelper
{
    /// <summary>
    /// Uploads a file to Storage.
    /// </summary>
    /// <param name="fileStream">The stream containing the file data.</param>
    /// <param name="fileName">The original name of the file.</param>
    /// <param name="folderName">The folder name within the Storage container where the file should be stored.</param>
    /// <returns>
    /// The URI of the uploaded file in Storage.
    /// </returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName);
}