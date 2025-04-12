namespace PeliculasAPI.Services
{
    public interface IFileStorage
    {
        Task<string> Storage(string container, IFormFile file);
        Task Delete(string container, string? path);
        async Task<string> Edit(string container, string? path, IFormFile file)
        {
            await Delete(container, path);
            return await Storage(container, file);
        }
    }
}
