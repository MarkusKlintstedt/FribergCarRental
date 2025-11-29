using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authenticationService;
        private readonly IMapper _mapper;
        private ApplicationUserService _applicationUserService;

        public AuthController(IAuthService authenticationService, IMapper mapper, ApplicationUserService applicationUserService)
        {
            this.authenticationService = authenticationService;
            _mapper = mapper;
            _applicationUserService = applicationUserService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginDto = new LoginUserDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var success = await authenticationService.AuthenticateAsync(loginDto);

            if (!success)
            {
                ModelState.AddModelError("", "Fel email eller lösenord");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await authenticationService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }


        // GET: Users/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Password,FirstName,LastName,Address,City,ZipCode,Email")] CreateUserViewModel userViewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(userViewModel.Password))
            {
                var user = _mapper.Map<CreateApplicationUserDto>(userViewModel);
                var addUserResult = await _applicationUserService.AddApplicationUser(user);
                if (addUserResult.Success == false)
                {
                    return NotFound();
                }
                return RedirectToAction("Index", "Home");
            }
            return View(userViewModel);
        }

    }
}

