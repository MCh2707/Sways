using Microsoft.AspNetCore.Components.Forms;

namespace Sways.Services;

public class FileUploadService
{
    private readonly IWebHostEnvironment _env;

    public FileUploadService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string?> UploadFileAsync(IBrowserFile file, string folder = "uploads")
    {
        if (file == null || file.Size == 0) return null;
        if (file.Size > 10 * 1024 * 1024) return null; 

        var targetFolder = Path.Combine(_env.WebRootPath, folder);
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        if (!allowedExts.Contains(ext))
        {
            ext = ".png";
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(targetFolder, fileName);

        await using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        await using var fs = new FileStream(fullPath, FileMode.Create);
        await stream.CopyToAsync(fs);

        return $"/{folder}/{fileName}";
    }
}
