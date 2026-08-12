using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Infrastructure.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageStorageService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary configuration is missing. Set Cloudinary:CloudName, Cloudinary:ApiKey and Cloudinary:ApiSecret.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<UploadedImageResult> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        if (fileStream is null)
            throw new ArgumentNullException(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(folder))
            folder = "seo-store";

        var extension = Path.GetExtension(fileName);
        var normalizedFileName = string.IsNullOrWhiteSpace(extension)
            ? fileName
            : Path.GetFileNameWithoutExtension(fileName);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = folder,
            PublicId = $"{normalizedFileName}-{Guid.NewGuid():N}",
            Overwrite = false,
            UseFilename = false,
            UniqueFilename = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        if (result.SecureUrl is null)
            throw new InvalidOperationException("Cloudinary returned an empty URL.");

        return new UploadedImageResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");
    }
}
