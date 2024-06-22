using Microsoft.AspNetCore.Http;

namespace Octagram.Application.Interfaces;

public interface IImageHelper
{
    /// <summary>
    /// Processes and uploads an image file to cloud storage.
    /// </summary>
    /// <param name="imageFile">The image file to be processed and uploaded.</param>
    /// <param name="folderName">The name of the folder in cloud storage to upload the image to.</param>
    /// <param name="cloudStorageHelper">An instance of the cloud storage helper for uploading the image.</param>
    /// <returns>
    /// The URL of the uploaded image.
    /// </returns>
    Task<string> ProcessAndUploadImageAsync(IFormFile imageFile, string folderName, ICloudStorageHelper cloudStorageHelper);
}