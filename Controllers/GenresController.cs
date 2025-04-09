using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController:  ControllerBase
    {
        private readonly IOutputCacheStore outputCacheStore;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private const string cacheTag = "genres";

        public GenresController(IOutputCacheStore outputCacheStore, ApplicationDBContext context, IMapper mapper)
        {
            this.outputCacheStore = outputCacheStore;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<GenreDTO>> Get()
        {
            var genres = await context.Genres.ProjectTo<GenreDTO>(mapper.ConfigurationProvider).ToListAsync();

            return genres;
        }

        [HttpGet("{id:int}", Name = "obtainById")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenreCreationDTO genreDTO)
        {
            Genre genre = mapper.Map<Genre>(genreDTO);

            context.Add(genre);
            await context.SaveChangesAsync();
            return CreatedAtRoute("obtainById", new {id = genre.Id}, genre);
        }

        [HttpPut]
        public void Put()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
