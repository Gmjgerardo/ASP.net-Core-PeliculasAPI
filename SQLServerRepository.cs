using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class SQLServerRepository : IRepository
    {
        private List<Genre> _genres;

        public SQLServerRepository()
        {
            _genres = new List<Genre>
            {
                new Genre{Id = 1, Name = "Comedia SQL"},
                new Genre{Id = 2, Name = "Acción SQL"},
            };
        }

        public bool Exist(string name)
        {
            return _genres.Any(g => g.Name == name);
        }

        public List<Genre> ObtainAllGenres()
        {
            return _genres;
        }

        public async Task<Genre?> ObtainGenreById(int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return _genres.FirstOrDefault(g => g.Id == id);
        }

        public void Create(Genre genre)
        {
            _genres.Add(genre);
        }
    }
}
