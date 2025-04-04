using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController:  ControllerBase
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
        [OutputCache]
        public async Task<ActionResult<Genre>> Get(int id)
        {
            var repository = new RepositorioEnMemoria();
            var genre = await repository.ObtainGenreById(id);

            return genre is null ? NotFound() : genre;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Genre genre)
        {
            var repository = new RepositorioEnMemoria();
            var genreNameIsRegistered = repository.Exist(genre.Name);

            if (genreNameIsRegistered)
            {
                return BadRequest($"Ya existe un género con el nombre {genre.Name}");
            }

            return Ok();
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
