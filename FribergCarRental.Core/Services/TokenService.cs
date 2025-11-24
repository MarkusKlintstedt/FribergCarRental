//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using FribergCarRental.Core.Classes;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;

//namespace FribergCarRental.Core.Services
//{
//    public class TokenService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
//        {
//            _configuration = configuration;
//            _userManager = userManager;
//        }

//        public async Task<string> GenerateTokenAsync(ApplicationUser user)
//        {
//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var roles = await _userManager.GetRolesAsync(user);
//            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

//            var userClaims = await _userManager.GetClaimsAsync(user);

//            var claims = new List<Claim>
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
//            new Claim("uid", user.Id)
//        }
//            .Union(roleClaims)
//            .Union(userClaims);

//            var token = new JwtSecurityToken(
//                issuer: _configuration["JwtSettings:Issuer"],
//                audience: _configuration["JwtSettings:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

//}
