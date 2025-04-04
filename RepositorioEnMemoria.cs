using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class RepositorioEnMemoria
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

        public List<Genre> ObtenerTodosLosGeneros()
        {
            return _genres;
        }

        public Genre? ObtainGenreById(int id)
        {
            return _genres.FirstOrDefault(g => g.Id == id);
        }
    }
}
