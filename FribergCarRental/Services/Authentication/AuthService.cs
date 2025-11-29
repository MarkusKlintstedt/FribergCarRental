using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FribergCarRental.Services.Base;

namespace FribergCarRental.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthService(IClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AuthenticateAsync(LoginUserDto loginUserDto)
        {
            var response = await httpClient.LoginAsync(loginUserDto);
            if (response == null || string.IsNullOrEmpty(response.Token))
                return false;

            httpContextAccessor.HttpContext.Response.Cookies.Append(
                "jwtToken", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

            return true;
        }

        public async Task LogoutAsync()
        {
            httpContextAccessor.HttpContext.Response.Cookies.Delete("jwtToken");
        }

        public string? GetToken()
        {
            return httpContextAccessor.HttpContext.Request.Cookies["jwtToken"];
        }


        public async Task<string?> GetUserId()
        {
            var token = httpContextAccessor.HttpContext.Request.Cookies["jwtToken"];
            if (token == null)
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return jwt.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
        }

        public ClaimsPrincipal? GetCurrentUser()
        {
            var token = GetToken();
            if (token == null)
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

        public string? GetUserName()
        {
            var user = GetCurrentUser();
            if (user == null) return null;

            return user.FindFirst(ClaimTypes.Name)?.Value
                ?? user.FindFirst("email")?.Value
                ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? user.FindFirst("uid")?.Value;
        }
    }

}

