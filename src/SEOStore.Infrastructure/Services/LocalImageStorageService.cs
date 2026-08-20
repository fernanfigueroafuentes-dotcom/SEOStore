using Microsoft.AspNetCore.Hosting;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalImageStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<UploadedImageResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string folder,
        string? assetName = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));

        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var safeFolder = SanitizeFolder(folder);
        var uploadsDirectory = Path.Combine(webRoot, "uploads", safeFolder);
        Directory.CreateDirectory(uploadsDirectory);

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || extension.Length > 8)
            extension = ".jpg";

        var baseName = ImageAssetNames.FromFileName(fileName, assetName);
        var storedFileName = baseName + extension.ToLowerInvariant();
        var filePath = Path.Combine(uploadsDirectory, storedFileName);
        var suffix = 2;
        while (File.Exists(filePath))
        {
            storedFileName = $"{baseName}-{suffix++}{extension.ToLowerInvariant()}";
            filePath = Path.Combine(uploadsDirectory, storedFileName);
        }

        await using (var output = File.Create(filePath))
        {
            await fileStream.CopyToAsync(output, cancellationToken);
        }

        var relativeUrl = $"/uploads/{safeFolder.Replace('\\', '/')}/{storedFileName}";
        return new UploadedImageResult(relativeUrl, $"local:{relativeUrl}");
    }

    public Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId) || !publicId.StartsWith("local:", StringComparison.Ordinal))
            return Task.CompletedTask;

        var relativeUrl = publicId["local:".Length..].TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var filePath = Path.GetFullPath(Path.Combine(webRoot, relativeUrl));
        var uploadsRoot = Path.GetFullPath(Path.Combine(webRoot, "uploads"));

        if (filePath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase) && File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    private static string SanitizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return "products";

        var parts = folder
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(SanitizeFileName)
            .Where(part => part.Length > 0);

        var safe = string.Join(Path.DirectorySeparatorChar, parts);
        return string.IsNullOrWhiteSpace(safe) ? "products" : safe;
    }

    private static string SanitizeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = value.Select(c => invalid.Contains(c) ? '-' : c).ToArray();
        var sanitized = new string(chars).Trim('.', ' ', '-');
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}
