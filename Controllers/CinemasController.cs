using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemasController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "cinemas";

        public CinemasController(ApplicationDBContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore)
            : base(context, mapper, outputCacheStore, cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [HttpGet("all")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<List<CinemaDTO>> Get([FromQuery] PaginationDTO pagination)
        {
            return await Get<Cinema, CinemaDTO>(pagination, orderParm: cinema => cinema.Name);
        }

        [HttpGet("{id:int}", Name = "obtainCinemaById")]
        [OutputCache(Tags =[cacheTag])]
        public async Task<ActionResult<CinemaDTO>> Get(int id)
        {
            return await Get<Cinema, CinemaDTO>(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CinemaCreationDTO creationDTO)
        {
            return await Post<Cinema, CinemaDTO, CinemaCreationDTO>(creationDTO, "obtainCinemaById");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CinemaCreationDTO cinemaUpdate)
        {
            return await Put<Cinema, CinemaCreationDTO>(id, cinemaUpdate);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await Delete<Cinema>(id);
        }
    }
}
