using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ApplicationUserController : ControllerBase
    {
        private readonly ApplicationUserRepository _applicationUserRepository;
        private readonly IMapper _mapper;

        public ApplicationUserController(ApplicationUserRepository applicationUserRepository, IMapper mapper)
        {
            _applicationUserRepository = applicationUserRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetAll()
        {
            var users = await _applicationUserRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<ApplicationUserDto>>(users);
            return Ok(userDtos.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDto>> GetById(string id)
        {
            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"No user with id {id} was found");
            }
            var userDto = _mapper.Map<ApplicationUserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] CreateApplicationUserDto newApplicationUserDto)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = newApplicationUserDto.Email,
                    Email = newApplicationUserDto.Email,
                    FirstName = newApplicationUserDto.FirstName,
                    LastName = newApplicationUserDto.LastName,
                    Address = newApplicationUserDto.Address,
                    City = newApplicationUserDto.City,
                    ZipCode = newApplicationUserDto.ZipCode
                };

                user.UserName = newApplicationUserDto.Email;
                var result = await _applicationUserRepository.CreateUserAsync(user, newApplicationUserDto.Password);

                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {nameof(AddUser)}", statusCode: 500);
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] EditApplicationUserDto applicationUserDto)
        {
            if (id != applicationUserDto.Id)
            {
                return BadRequest("Id mismatch");
            }

            var existingUser = await _applicationUserRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            _mapper.Map(applicationUserDto, existingUser);
            await _applicationUserRepository.UpdateUserAsync(existingUser);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _applicationUserRepository.DeleteUserAsync(user);
            return Ok();
        }
    }
}
