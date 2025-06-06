using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IFileStorage storage;
        private const string cacheTag = "movies";
        private const string container = "movies";

        public MoviesController(ApplicationDBContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore, IFileStorage storage)
            : base(context, mapper, outputCacheStore, cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.storage = storage;
        }

        [HttpGet("landing")]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            int top = 6;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            List<MovieDTO> upcomingMovies = await context.Movies
                .Where(m => m.ReleaseDate > today)
                .OrderBy(m => m.ReleaseDate)
                .Take(top)
                .ProjectTo<MovieDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            List<MovieDTO> onCinemasMovies = await context.Movies
                .Where(movie => movie.MovieCinemas.Select(mc => mc.MovieId).Contains(movie.Id))
                .OrderBy(m => m.ReleaseDate)
                .Take(top)
                .ProjectTo<MovieDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            LandingPageDTO result = new LandingPageDTO();

            result.Upcoming = upcomingMovies;
            result.OnCinemas = onCinemasMovies;

            return result;
        }

        [HttpGet("{id:int}", Name = "obtainMovieById")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            ActionResult<MovieDetailsDTO> result = NotFound();

            MovieDetailsDTO? movie = await context.Movies
                .ProjectTo<MovieDetailsDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is not null)
                result = movie;

            return result;
        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetDTO>> PostGet()
        {
            List<CinemaDTO> cinemas = await context.Cinemas.ProjectTo<CinemaDTO>(mapper.ConfigurationProvider).ToListAsync();
            List<GenreDTO> genres = await context.Genres.ProjectTo<GenreDTO>(mapper.ConfigurationProvider).ToListAsync();

            return new MoviePostGetDTO
            {
                Cinemas = cinemas,
                Genres = genres,
            };
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<MovieActorDTO>>> Get(string name)
        {
            return await context.Actors.Where(a => a.Name.Contains(name))
                .ProjectTo<MovieActorDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromForm] MovieCreationDTO movieCreation)
        {
            Movie movie = mapper.Map<Movie>(movieCreation);

            if (movieCreation.Image is not null)
            {
                string? url = await storage.Storage(container, movieCreation.Image);
                movie.Image = url;
            }

            AssignActorsOrder(movie);
            context.Add(movie);

            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default);

            MovieDTO? movieDTO = mapper.Map<MovieDTO>(movie);

            return CreatedAtRoute("obtainMovieById", new { id = movie.Id }, movieDTO);
        }

        private void AssignActorsOrder(Movie movie)
        {
            if (movie.MovieActors is not null)
            {
                for (global::System.Int32 i = 0; i < movie.MovieActors.Count; i++)
                {
                    movie.MovieActors[i].Order = i;
                }
            }
        }
    }
}
