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
    public class GenresController:  CustomBaseController
    {
        private readonly IOutputCacheStore outputCacheStore;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private const string cacheTag = "genres";

        public GenresController(IOutputCacheStore outputCacheStore, ApplicationDBContext context, IMapper mapper)
            : base(context, mapper)
        {
            this.outputCacheStore = outputCacheStore;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<GenreDTO>> Get([FromQuery] PaginationDTO pagination)
        {
            return await Get<Genre, GenreDTO>(pagination, orderParm: genre => genre.Name);
        }

        [HttpGet("{id:int}", Name = "obtainGenreById")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            return await Get<Genre, GenreDTO>(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenreCreationDTO genreDTO)
        {
            Genre genre = mapper.Map<Genre>(genreDTO);

            context.Add(genre);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("obtainGenreById", new {id = genre.Id}, genre);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] GenreCreationDTO genreUpdate)
        {
            IActionResult response = NotFound();
            Boolean genreExist = await context.Genres.AnyAsync(g => g.Id == id);

            if (genreExist is true)
            {
                Genre genre = mapper.Map<Genre>(genreUpdate);
                genre.Id = id;

                context.Update(genre);
                await context.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync(cacheTag, default);

                response = NoContent();
            }

            return response;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult response = NotFound();
            int deletedRows = await context.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();

            if (deletedRows > 0)
            {
                await outputCacheStore.EvictByTagAsync(cacheTag, default);
                response = NoContent();
            }
            
            return response;
        }
    }
}
