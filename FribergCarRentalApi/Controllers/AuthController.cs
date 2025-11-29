using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(ApplicationUserRepository applicationUserRepository, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _applicationUserRepository = applicationUserRepository;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
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

                string tokenstring = await GenerateToken(user);

                var response = new AuthResponse
                {
                    Email = user.Email,
                    Token = tokenstring,
                    UserId = user.Id
                };

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


    }
}
