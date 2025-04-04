using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    public class GenresController
    {
        [HttpGet]
        [HttpGet("all")]
        public List<Genre> Get()
        {
            var repository = new RepositorioEnMemoria();
            var genres = repository.ObtenerTodosLosGeneros();

            return genres;
        }

        [HttpGet("{id:int}")]
        public Genre? Get(int id)
        {
            var repository = new RepositorioEnMemoria();
            var genre = repository.ObtainGenreById(id);

            return genre;
        }

        [HttpPost]
        public void Post()
        {

        }

        [HttpPut]
        public void Put()
        {

        }

        [HttpDelete]
        public void Delete()
        {

        }
    }
}
