using System.Security.Claims;
using FribergCarRental.Services.Base;

namespace FribergCarRental.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> AuthenticateAsync(LoginUserDto loginModel);
        ClaimsPrincipal? GetCurrentUser();
        Task<string?> GetUserId();
        string? GetUserName();
        public Task LogoutAsync();
    }
}
