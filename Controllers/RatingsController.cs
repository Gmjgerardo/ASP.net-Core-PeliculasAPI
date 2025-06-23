using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IUsersService usersService;

        public RatingsController(ApplicationDBContext context, IUsersService usersService)
        {
            this.context = context;
            this.usersService = usersService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] RatingCreationDTO creationDTO)
        {
            string userId = await usersService.GetUserId();

            Rating? actualRating = context.MovieRatings
                .FirstOrDefault(r => r.UserId == userId && r.MovieId == creationDTO.MovieId);

            try
            {
                if (actualRating is null)
                    context.Add<Rating>(new Rating()
                    {
                        MovieId = creationDTO.MovieId,
                        UserId = userId,
                        Rate = creationDTO.Rate,
                    });
                else
                    actualRating.Rate = creationDTO.Rate;

                await context.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Error interno del servidor",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable
                    );
            }
        }
    }
}
