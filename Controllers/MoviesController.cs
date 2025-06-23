using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Services;
using PeliculasAPI.Utilities;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IFileStorage storage;
        private readonly IUsersService usersService;
        private const string cacheTag = "movies";
        private const string container = "movies";

        public MoviesController(ApplicationDBContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore, IFileStorage storage, IUsersService usersService)
            : base(context, mapper, outputCacheStore, cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.storage = storage;
            this.usersService = usersService;
        }

        [HttpGet("landing")]
        [OutputCache(Tags = [cacheTag])]
        [AllowAnonymous]
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
        //[OutputCache(Tags = [cacheTag])]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            ActionResult<MovieDetailsDTO> result = NotFound();

            MovieDetailsDTO? movie = await context.Movies
                .ProjectTo<MovieDetailsDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is not null)
            {
                double averageRating = 0.0;
                int userRate = 0;

                // Retrieve rating data for the movie
                if (await context.MovieRatings.AnyAsync(rate => rate.MovieId == id))
                {
                    averageRating = await context.MovieRatings.Where(rate => rate.MovieId == id)
                        .AverageAsync(r => r.Rate);
                }

                // Modify User Movie Rate if is logged
                if (HttpContext.User.Identity!.IsAuthenticated)
                {
                    string userId = await usersService.GetUserId();
                    Rating? dbRate = await context.MovieRatings
                        .FirstOrDefaultAsync(r => r.MovieId == id
                        && r.UserId == userId);

                    if (dbRate is not null)
                        userRate = dbRate.Rate;
                }

                movie.AverageRating = averageRating;
                movie.UserRate = userRate;
                result = movie;
            }

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

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] MovieFilterDTO filterDTO)
        {
            IQueryable<Movie> movieQueryable = context.Movies.AsQueryable();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (!string.IsNullOrWhiteSpace(filterDTO.Title))
                movieQueryable = movieQueryable.Where(m => m.Title.ToLower().Contains(filterDTO.Title.ToLower()));

            if (filterDTO.OnCinemas)
                movieQueryable = movieQueryable.Where(m => m.MovieCinemas.Count() > 0);

            if (filterDTO.ComingSoon)
                movieQueryable = movieQueryable.Where(m => m.ReleaseDate > today);

            if (filterDTO.GenreId != 0)
                movieQueryable = movieQueryable.Where(m => m.MovieGenres
                .Select(g => g.GenreId).Contains(filterDTO.GenreId));

            await HttpContext.InsertPaginationParametersOnHeader(movieQueryable);

            if (filterDTO.Pagination.RowsPerPage != 0 & filterDTO.Pagination.Page != 0)
                movieQueryable = movieQueryable.Paginate(filterDTO.Pagination);

            return await movieQueryable
                .ProjectTo<MovieDTO>(mapper.ConfigurationProvider)
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Obtaining image/poster information from the selectes movie
            string? imagePath = await context.Movies
                .Where(movie => movie.Id == id)
                .Select(movie => movie.Image)
                .FirstOrDefaultAsync();

            IActionResult response = await Delete<Movie>(id);

            // Deleting image from storage if the movie was deleted correctly
            if (response is NoContentResult)
                await storage.Delete(container, imagePath);

            return response;
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
