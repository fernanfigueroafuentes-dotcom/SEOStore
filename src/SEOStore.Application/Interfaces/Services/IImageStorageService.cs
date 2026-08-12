namespace SEOStore.Application.Interfaces.Services;

public interface IImageStorageService
{
    Task<UploadedImageResult> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}

public record UploadedImageResult(string Url, string PublicId);
