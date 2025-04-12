
namespace PeliculasAPI.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Storage(string container, IFormFile file)
        {
            HttpRequest request = httpContextAccessor.HttpContext!.Request!;
            string extension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string dir = Path.Combine(env.WebRootPath, container);
            string path = Path.Combine(dir, fileName);
            string url = $"{request.Scheme}://{request.Host}";

            // Create directory if not exists
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (MemoryStream ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                Byte[] content = ms.ToArray();

                await File.WriteAllBytesAsync(path, content);
            }

            var formattedUrl = Path.Combine(url, container, fileName).Replace("\\", "/");
            return formattedUrl;
        }

        public Task Delete(string container, string? path)
        {
            if (string.IsNullOrWhiteSpace(path) is false)
            {
                string fileName = Path.GetFileName(path);
                string filePath = Path.Combine(env.WebRootPath, container, fileName);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
