using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FribergCarRental.Middleware
{
    public class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var jwt = context.Request.Cookies["jwtToken"];

            if (!string.IsNullOrEmpty(jwt))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var identity = new ClaimsIdentity(token.Claims, "jwtAuth");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}
