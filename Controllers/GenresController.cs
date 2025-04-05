using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController:  ControllerBase
    {
        private readonly IRepository repository;
        private readonly TransientService transient1;
        private readonly TransientService transient2;
        private readonly ScopedService scoped1;
        private readonly ScopedService scoped2;
        private readonly SingletonService singleton;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IConfiguration configuration;
        private const string cacheTag = "genres";

        public GenresController(IRepository repository,
            TransientService transient1,
            TransientService transient2,
            ScopedService scoped1,
            ScopedService scoped2,
            SingletonService singleton,
            IOutputCacheStore outputCacheStore,
            IConfiguration configuration
            )
        {
            this.repository = repository;
            this.transient1 = transient1;
            this.transient2 = transient2;
            this.scoped1 = scoped1;
            this.scoped2 = scoped2;
            this.singleton = singleton;
            this.outputCacheStore = outputCacheStore;
            this.configuration = configuration;
        }

        [HttpGet("ejemplo-proveedor-configuracion")]
        public string GetConnectionStringProvider()
        {
            return configuration.GetValue<string>("conectionString")!;
        }

        [HttpGet("servicios-tiempos-de-vida")]
        public IActionResult GetLifeTimeServices()
        {
            return Ok(new
            {
                transients = new { transient1 = transient1.getId, transient2 = transient2.getId },
                scopeds = new { scoped1 = scoped1.getId, scoped2 = scoped2.getId },
                singleton = singleton.getId,
            });
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public List<Genre> Get()
        {
            var genres = repository.ObtainAllGenres();

            return genres;
        }

        [HttpGet("{id:int}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Genre>> Get(int id)
        {   
            var genre = await repository.ObtainGenreById(id);

            return genre is null ? NotFound() : genre;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Genre genre)
        {   
            var genreNameIsRegistered = repository.Exist(genre.Name);

            if (genreNameIsRegistered)
            {
                return BadRequest($"Ya existe un género con el nombre {genre.Name}");
            }

            repository.Create(genre);
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

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
