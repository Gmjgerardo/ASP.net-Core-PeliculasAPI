using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    public class GenresController
    {
        [HttpGet]
        public List<Genre> Get()
        {
            var repository = new RepositorioEnMemoria();
            var genres = repository.ObtenerTodosLosGeneros();

            return genres;
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
