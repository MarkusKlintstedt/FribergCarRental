using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.Core.Services;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IConfiguration _configuration;

        //public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        //{
        //    _userManager = userManager;
        //    _configuration = configuration;
        //}

        public ApplicationUserRepository _applicationUserRepository { get; set; }
        public IMapper _mapper { get; set; }
        public TokenService _tokenService { get; set; }

        public AuthController(ApplicationUserRepository applicationUserRepository, IMapper mapper, TokenService tokenService)
        {
            _applicationUserRepository = applicationUserRepository;
            _mapper = mapper;
            _tokenService = tokenService;
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

                string tokenstring = await _tokenService.GenerateTokenAsync(user);

                var response = new AuthResponse
                {
                    Email = user.Email,
                    Token = tokenstring,
                    UserId = user.Id
                };

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,           // krävs för SameSite=None i moderna browsers
                    SameSite = SameSiteMode.None,
                    Path = "/"
                    // Domain = "localhost" // undvik på localhost; låt serverns origin bestämma
                };
                Response.Cookies.Append("jwtToken", tokenstring, cookieOptions);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

    }
}
