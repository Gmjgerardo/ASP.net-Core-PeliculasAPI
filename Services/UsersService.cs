
using Microsoft.AspNetCore.Identity;

namespace PeliculasAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;

        public UsersService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<string> GetUserId()
        {
            string email = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == "email")!.Value;
            IdentityUser? user = await userManager.FindByEmailAsync(email);
            return user!.Id;
        }
    }
}
