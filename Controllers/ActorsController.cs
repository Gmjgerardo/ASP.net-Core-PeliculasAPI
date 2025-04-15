using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Services;
using PeliculasAPI.Utilities;

namespace PeliculasAPI.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorsController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IFileStorage fileStorage;
        private const string cacheTag = "actors";
        private const string container = "actors";

        public ActorsController(ApplicationDBContext context, IMapper mapper,
                IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<ActorDTO>> Get([FromQuery] PaginationDTO pagination)
        {
            DbSet<Actor> queryable = context.Actors;

            await HttpContext.InsertPaginationParametersOnHeader(queryable);
            return await queryable
                .OrderBy(actor => actor.Name)
                .Paginate(pagination)
                .ProjectTo<ActorDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        [HttpGet("{id:int}", Name = "obtainActorById")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            ActorDTO? actor = await context.Actors
                .ProjectTo<ActorDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(actor => actor.Id == id);

            return (actor is null) ? NotFound() : actor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ActorCreationDTO actorCreation)
        {
            Actor actor = mapper.Map<Actor>(actorCreation);

            if (actorCreation.ProfileImage is not null)
            {
                string url = await fileStorage.Storage(container, actorCreation.ProfileImage);
                actor.ProfileImage = url;
            }

            context.Add(actor);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);
            
            return CreatedAtRoute("obtainActorById", new {id = actor.Id}, actor);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] ActorCreationDTO actorNewData)
        {
            IActionResult response = NotFound();
            Actor? actor = await context.Actors.SingleOrDefaultAsync(actor => actor.Id == id);

            if (actor is not null)
            {
                actor = mapper.Map<ActorCreationDTO, Actor>(actorNewData, actor);

                // Edit profile image
                if (actorNewData.ProfileImage is not null)
                {
                    string newPath = await fileStorage.Edit(
                        container,
                        path: actor.ProfileImage,
                        file: actorNewData.ProfileImage);

                    actor.ProfileImage = newPath;
                }

                context.Update(actor);
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
            Actor? actor = await context.Actors.FirstOrDefaultAsync(actor => actor.Id == id);

            if (actor is not null)
            {
                context.Remove<Actor>(actor);

                await fileStorage.Delete(container, actor.ProfileImage);
                await context.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync(cacheTag, default);
                response = NoContent();
            }

            return response;
        }
    }
}
