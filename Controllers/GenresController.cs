using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Utilities;

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
        public async Task<List<GenreDTO>> Get([FromQuery] PaginationDTO pagination)
        {
            DbSet<Genre> queryable = context.Genres;
            await HttpContext.InsertPaginationParametersOnHeader(queryable);
            List<GenreDTO> genres = await queryable
                .OrderBy(g=> g.Id)
                .Paginate(pagination)
                .ProjectTo<GenreDTO>(mapper.ConfigurationProvider).ToListAsync();

            return genres;
        }

        [HttpGet("{id:int}", Name = "obtainById")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            GenreDTO? genre = await context.Genres
                .ProjectTo<GenreDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(g => g.Id == id);
            
            return (genre is null) ? NotFound() : genre;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenreCreationDTO genreDTO)
        {
            Genre genre = mapper.Map<Genre>(genreDTO);

            context.Add(genre);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("obtainById", new {id = genre.Id}, genre);
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

        [HttpDelete]
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
