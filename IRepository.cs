using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public interface IRepository
    {
        public List<Genre> ObtainAllGenres();

        public Task<Genre?> ObtainGenreById(int id);

        public bool Exist(string name);
    }
}
