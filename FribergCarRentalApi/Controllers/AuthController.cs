using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public ApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public IMapper _mapper { get; set; }
        private readonly ApplicationDbContext _context;


        public AuthController(ApplicationUserRepository applicationUserRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _applicationUserRepository = applicationUserRepository;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] CreateApplicationUserDto applicationUserDto)
        {
            try
            {
                var user = _mapper.Map<ApplicationUser>(applicationUserDto);
                user.UserName = applicationUserDto.Email;
                var result = await _applicationUserRepository.CreateUserAsync(user, applicationUserDto.Password);

                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Accepted();

            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _applicationUserRepository.GetByEmailAsync(loginUserDto.Email);
                if (user == null)
                {
                    return Unauthorized(loginUserDto);
                }


                var passwordValid = await _applicationUserRepository.CheckPasswordAsync(user, loginUserDto.Password);
                if (passwordValid == false)
                {
                    return Unauthorized(loginUserDto);
                }

                string tokenstring = await GenerateToken(user);  // _tokenService.GenerateTokenAsync(user);

                var response = new AuthResponse
                {
                    Email = user.Email,
                    Token = tokenstring,
                    UserId = user.Id
                };

                var refreshToken = new RefreshToken
                {
                    Token = GenerateRefreshToken(),
                    UserId = user.Id,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(roleClaims)
            .Union(userClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<RefreshToken> SaveRefreshToken(ApplicationUser user)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> ValidateRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(x => x.UserId)
                .FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
                return null;

            return refreshToken;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRequest model)
        {
            var refreshToken = await ValidateRefreshToken(model.RefreshToken);

            if (refreshToken == null)
                return Unauthorized("Invalid refresh token");

            var user = await _applicationUserRepository.GetByIdAsync(refreshToken.UserId);

            var newAccessToken = await GenerateAccessToken(user);
            var newRefreshToken = await SaveRefreshToken(user);

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        private async Task<string> GenerateAccessToken(ApplicationUser? user)
        {
            var securityKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"])
    );

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("uid", user.Id)
    }
            .Union(roleClaims)
            .Union(userClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
