using Microsoft.Extensions.Logging;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Infrastructure.Services;

public sealed class FallbackImageStorageService : IImageStorageService
{
    private readonly IImageStorageService? _primary;
    private readonly IImageStorageService _fallback;
    private readonly ILogger<FallbackImageStorageService> _logger;

    public FallbackImageStorageService(
        IImageStorageService? primary,
        IImageStorageService fallback,
        ILogger<FallbackImageStorageService> logger)
    {
        _primary = primary;
        _fallback = fallback;
        _logger = logger;
    }

    public async Task<UploadedImageResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder,
        string? assetName = null,
        CancellationToken cancellationToken = default)
    {
        await using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        var payload = buffer.ToArray();

        if (_primary is not null)
        {
            try
            {
                await using var cloudinaryStream = new MemoryStream(payload, writable: false);
                return await _primary.UploadAsync(cloudinaryStream, fileName, folder, assetName, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Cloudinary upload failed. The image will be stored locally.");
            }
        }

        await using var localStream = new MemoryStream(payload, writable: false);
        return await _fallback.UploadAsync(localStream, fileName, folder, assetName, cancellationToken);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        if (publicId.StartsWith("local:", StringComparison.Ordinal))
        {
            await _fallback.DeleteAsync(publicId, cancellationToken);
            return;
        }

        if (_primary is not null)
            await _primary.DeleteAsync(publicId, cancellationToken);
    }
}
