using PeliculasAPI.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace PeliculasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Register(UserCredentialsDTO userCredentials)
        {
            ActionResult<AuthenticationResponseDTO> response = NotFound();
            IdentityUser user = new IdentityUser
            {
                Email = userCredentials.Email,
                UserName = userCredentials.Email,
            };

            IdentityResult result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
                response = await BuildToken(user);
            else
                response = BadRequest(result.Errors);

            return response;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login(UserCredentialsDTO userCredentials)
        {
            ActionResult<AuthenticationResponseDTO> response = BadRequest(new List<IdentityError>{ new IdentityError { Description = "Login incorrecto" } });
            IdentityUser? user = await userManager.FindByEmailAsync(userCredentials.Email);

            if (user is not null)
            {
                Microsoft.AspNetCore.Identity.SignInResult result =
                    await signInManager.CheckPasswordSignInAsync(user, userCredentials.Password, lockoutOnFailure: false);

                if (result.Succeeded)
                    response = await BuildToken(user);
            }

            return response;
        }

        private async Task<AuthenticationResponseDTO> BuildToken(IdentityUser identityUser)
        {
            List<Claim> claims = new List<Claim> { new Claim("email", identityUser.Email!) };
            IList<Claim> claimsDB = await userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtKey"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expirationTime = DateTime.UtcNow.AddDays(2);

            JwtSecurityToken securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expirationTime, signingCredentials: creds);
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO { Token = token, Expiration = expirationTime };
        }
    }
}
