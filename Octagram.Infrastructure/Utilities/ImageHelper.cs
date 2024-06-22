using Microsoft.AspNetCore.Http;
using Octagram.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Octagram.Infrastructure.Utilities;

public class ImageHelper : IImageHelper
{
    /// <summary>
    /// Processes and uploads an image file to cloud storage.
    /// </summary>
    /// <param name="imageFile">The image file to be processed and uploaded.</param>
    /// <param name="folderName">The name of the folder in cloud storage to upload the image to.</param>
    /// <param name="cloudStorageHelper">An instance of the cloud storage helper for uploading the image.</param>
    /// <returns>
    /// The URL of the uploaded image in cloud storage.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the provided image file is invalid.</exception>
    public async Task<string> ProcessAndUploadImageAsync(IFormFile imageFile, string folderName,
        ICloudStorageHelper cloudStorageHelper)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentException("Invalid image file.");
        }

        using var image = await Image.LoadAsync(imageFile.OpenReadStream());

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(800, 0)
        }));


        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder());

        outputStream.Position = 0;
        var imageUrl = await cloudStorageHelper.UploadFileAsync(outputStream,
            imageFile.FileName,
            folderName);
        return imageUrl;
    }
}