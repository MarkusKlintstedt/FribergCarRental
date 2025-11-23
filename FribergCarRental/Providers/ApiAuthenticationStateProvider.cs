//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Components.Authorization;

//namespace FribergCarRental.Providers
//{
//    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
//    {
//        private readonly ILocalStorageService localStorage;
//        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;
//        private readonly IHttpContextAccessor httpContextAccessor;

//        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, IHttpContextAccessor httpContextAccessor)
//        {
//            this.localStorage = localStorage;
//            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
//            this.httpContextAccessor = httpContextAccessor;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            var user = new ClaimsPrincipal(new ClaimsIdentity());
//            var savedToken = httpContextAccessor?.HttpContext?.Request?.Cookies?["accessToken"]; //await localStorage.GetItemAsync<string>("accessToken");
//            if (savedToken == null)
//            {
//                return new AuthenticationState(user);
//            }

//            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(savedToken);

//            if (tokenContent.ValidTo < DateTime.Now)
//            {
//                return new AuthenticationState(user);
//            }

//            var claims = await GetClaims();
//            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
//            return new AuthenticationState(user);

//        }

//        public async Task LoggedIn()
//        {
//            var claims = await GetClaims();
//            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
//            var authState = Task.FromResult(new AuthenticationState(user));
//            NotifyAuthenticationStateChanged(authState);
//        }

//        public async Task LoggedOut()
//        {
//            await localStorage.RemoveItemAsync("accessToken");
//            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
//            var authState = Task.FromResult(new AuthenticationState(nobody));
//            NotifyAuthenticationStateChanged(authState);
//        }



//        private async Task<List<Claim>> GetClaims()
//        {
//            var savedToken = httpContextAccessor?.HttpContext?.Request?.Cookies?["accessToken"];//await localStorage.GetItemAsync<string>("accessToken");
//            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(savedToken);
//            var claims = tokenContent.Claims.ToList();
//            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
//            return claims;
//        }

//    }
//}
