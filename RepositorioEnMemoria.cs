using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class RepositorioEnMemoria: IRepository
    {
        private List<Genre> _genres;

        public RepositorioEnMemoria()
        {
            _genres = new List<Genre>
            {
                new Genre {Id = 1, Name = "Comedia"},
                new Genre {Id = 2, Name = "Acción"},
            };
        }

        public List<Genre> ObtainAllGenres()
        {
            return _genres;
        }

        public async Task<Genre?> ObtainGenreById(int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            return _genres.FirstOrDefault(g => g.Id == id);
        }

        public bool Exist(string name)
        {
            return _genres.Any(g => g.Name == name);
        }

        public void Create(Genre genre)
        {
            _genres.Add(genre);
        }
    }
}
