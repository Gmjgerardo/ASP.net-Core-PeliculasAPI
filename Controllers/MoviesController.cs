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
        [OutputCache(Tags = [cacheTag])]
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
        [OutputCache(Tags = [cacheTag])]
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

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
        {
            ActionResult<MoviePutGetDTO> result = NotFound();

            MovieDetailsDTO? movie = await context.Movies
                .ProjectTo<MovieDetailsDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is not null) {
                MoviePutGetDTO moviePutGet = new MoviePutGetDTO();

                List<int> selectedGenresIds = movie.Genres.Select(g => g.Id).ToList();
                List<GenreDTO> notSelectedGenres = await context.Genres
                    .Where(g => !selectedGenresIds.Contains(g.Id))
                    .ProjectTo<GenreDTO>(mapper.ConfigurationProvider)
                    .ToListAsync();

                List<int> selectedCinemasIds = movie.Cinemas.Select(c => c.Id).ToList();
                List<CinemaDTO> notSelectedCinemas = await context.Cinemas
                    .Where(c => !selectedCinemasIds.Contains(c.Id))
                    .ProjectTo<CinemaDTO>(mapper.ConfigurationProvider)
                    .ToListAsync();

                moviePutGet.Movie = movie;
                moviePutGet.SelectedGenres = movie.Genres;
                moviePutGet.NotSelectedGenres = notSelectedGenres;
                moviePutGet.SelectedCinemas = movie.Cinemas;
                moviePutGet.NotSelectedCinemas = notSelectedCinemas;
                moviePutGet.Actors = movie.Actors;

                result = moviePutGet;
            }

            return result;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreateDTO)
        {
            IActionResult result = NotFound();

            Movie? movie = await context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieCinemas)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is not null)
            {
                movie = mapper.Map(movieCreateDTO, movie);

                if (movieCreateDTO.Image is not null)
                {
                    movie.Image = await storage.Edit(container, movie.Image, movieCreateDTO.Image);
                }

                AssignActorsOrder(movie);

                await context.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync(cacheTag, default);

                result = NoContent();
            }

            return result;
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
