using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Infrastructure.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string? _uploadPreset;

    public CloudinaryImageStorageService(IConfiguration configuration)
    {
        if (!CloudinarySettings.TryRead(configuration, out var cloudName, out var apiKey, out var apiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing. Set CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY and CLOUDINARY_API_SECRET in the .env file.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
        _uploadPreset = configuration["CLOUDINARY_UPLOAD_PRESET"]?.Trim();
    }

    public async Task<UploadedImageResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder,
        string? assetName = null,
        CancellationToken cancellationToken = default)
    {
        if (fileStream is null)
            throw new ArgumentNullException(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        if (string.IsNullOrWhiteSpace(folder))
            folder = "seo-store";

        await using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var publicId = ImageAssetNames.FromFileName(fileName, assetName);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, buffer),
            Folder = folder,
            PublicId = publicId,
            Overwrite = false,
            UseFilename = false,
            UniqueFilename = false,
            Transformation = new Transformation()
                .Width(720)
                .Height(900)
                .Crop("limit")
                .Quality("auto:good")
        };

        if (!string.IsNullOrWhiteSpace(_uploadPreset))
        {
            uploadParams.UploadPreset = _uploadPreset;
            uploadParams.Unsigned = true;
        }

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException(FormatUploadError(result.Error.Message));

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

    private static string FormatUploadError(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Cloudinary upload failed.";

        if (message.Contains("missing permissions", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("actions=[\"create\"]", StringComparison.OrdinalIgnoreCase))
        {
            return "Cloudinary upload failed: the API key does not have Create/Upload permission. " +
                   "In console.cloudinary.com go to Settings > API Keys, edit this key and assign Master Admin " +
                   "(or a role that can upload assets). Then restart the app.";
        }

        return $"Cloudinary upload failed: {message}";
    }
}
