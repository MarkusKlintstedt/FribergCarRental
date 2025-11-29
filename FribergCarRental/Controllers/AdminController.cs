using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IMapper _mapper;
        private BookingService _bookingService;
        private CarService _carService;
        private ApplicationUserService _applicationUserService;

        public AdminController(IMapper mapper, CarService carRepository,
            BookingService bookingRepository, ApplicationUserService applicationUserRepository, IAuthService authService) : base(authService)
        {
            _mapper = mapper;
            _bookingService = bookingRepository;
            _carService = carRepository;
            _applicationUserService = applicationUserRepository;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Cars()
        {
            var carsResponse = await _carService.GetCars();
            var carViewModels = _mapper.Map<List<CarViewModel>>(carsResponse.Data);
            return View(carViewModels);
        }

        public IActionResult CreateCar()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar([Bind("CarId,Brand,ModelName,Description,RentPricePerDay")] CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                var car = _mapper.Map<CarDto>(carViewModel);
                await _carService.AddCar(car);
                return RedirectToAction(nameof(Cars));
            }
            return View(carViewModel);
        }

        public async Task<IActionResult> EditCar(int id)
        {
            var carResponse = await _carService.GetCar(id);
            if (carResponse.Success == false)
                return HandleApiError(carResponse.Message, carResponse.ValidationErrors);
            var carViewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            return View(carViewModel);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, [Bind("CarId,Brand,ModelName,Description,RentPricePerDay")] CarViewModel carViewModel)
        {
            if (id != carViewModel.CarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var car = _mapper.Map<CarDto>(carViewModel);
                var response = await _carService.UpdateCar(car.CarId, car);
                if (response.Success == false)
                    return HandleApiError(response.Message, response.ValidationErrors);
                return RedirectToAction(nameof(Cars));
            }
            return View(carViewModel);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> DeleteCar(int id)
        {
            var carResponse = await _carService.GetCar(id);
            if (carResponse.Success == false)
                return HandleApiError(carResponse.Message, carResponse.ValidationErrors);
            var carViewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            return View(carViewModel);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("DeleteCar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCar(int id)
        {
            var deleteResponse = await _carService.DeleteCar(id);
            if (deleteResponse.Success == false)
                return HandleApiError(deleteResponse.Message, deleteResponse.ValidationErrors);
            return RedirectToAction(nameof(Cars));
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Bookings()
        {
            var bookingsResponse = await _bookingService.GetAll();
            if (bookingsResponse.Success == false)
                return HandleApiError(bookingsResponse.Message, bookingsResponse.ValidationErrors);
            var bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookingsResponse.Data);
            return View(bookingViewModels);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);
            if (bookingResponse.Success == false)
                return HandleApiError(bookingResponse.Message, bookingResponse.ValidationErrors);
            var bookingViewModel = _mapper.Map<BookingViewModel>(bookingResponse.Data);
            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Delete/5
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookingConfirmed(int id)
        {
            var deleteResponse = await _bookingService.Delete(id);
            if (deleteResponse.Success == false)
                return HandleApiError(deleteResponse.Message, deleteResponse.ValidationErrors);
            return RedirectToAction(nameof(Bookings));
        }


        // GET: Users
        public async Task<IActionResult> Users()
        {
            var usersResponse = await _applicationUserService.GetApplicationUsers();
            if (usersResponse.Success == false)
                return HandleApiError(usersResponse.Message, usersResponse.ValidationErrors);
            var userViewModels = _mapper.Map<List<UserViewModel>>(usersResponse.Data);
            return View(userViewModels);
        }

        // GET: Users/Create
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Password,FirstName,LastName,Address,City,ZipCode,Email")] CreateUserViewModel userViewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(userViewModel.Password))
            {
                var user = _mapper.Map<CreateApplicationUserDto>(userViewModel);
                var addUserResponse = await _applicationUserService.AddApplicationUser(user);
                if (addUserResponse.Success == false)
                    return HandleApiError(addUserResponse.Message, addUserResponse.ValidationErrors);
                return RedirectToAction(nameof(Users));
            }
            return View(userViewModel);
        }

        public async Task<IActionResult> EditUser(string? id)
        {
            var userResponse = await _applicationUserService.GetApplicationUser(id);
            if (userResponse.Success == false)
                return HandleApiError(userResponse.Message, userResponse.ValidationErrors);
            var userViewModel = _mapper.Map<UserViewModel>(userResponse.Data);
            return View(userViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(String id, [Bind("Id,FirstName,LastName,Address,City,ZipCode,Email")] UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            var editedUser = _mapper.Map<EditApplicationUserDto>(userViewModel);

            var userResponse = await _applicationUserService.GetApplicationUser(id);
            if (userResponse.Success == false)
                return HandleApiError(userResponse.Message, userResponse.ValidationErrors);
            var response = await _applicationUserService.UpdateApplicationUser(userResponse.Data.Id, editedUser);
            if (response.Success == true)
                return HandleApiError(response.Message, response.ValidationErrors);
            _mapper.Map(userViewModel, userResponse.Data);
            return View(userViewModel);
        }


        // GET: User/Delete/5
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userResponse = await _applicationUserService.GetApplicationUser(id);
            if (userResponse.Success == false)
                return HandleApiError(userResponse.Message, userResponse.ValidationErrors);
            var userViewModel = _mapper.Map<UserViewModel>(userResponse.Data);
            return View(userViewModel);
        }

        // POST: Users/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUser(string id)
        {
            var deleteUserResponse = await _applicationUserService.DeleteApplicationUser(id);
            if (deleteUserResponse.Success == false)
                return HandleApiError(deleteUserResponse.Message, deleteUserResponse.ValidationErrors);
            return RedirectToAction(nameof(Users));
        }


        public async Task<IActionResult> Images(int id)
        {
            var carResponse = await _carService.GetCar(id);
            if (carResponse.Success == false)
                return HandleApiError(carResponse.Message, carResponse.ValidationErrors);
            var viewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            return View(viewModel);
        }

        public async Task<IActionResult> AddImage(int id, string newImagePath)
        {
            if (string.IsNullOrEmpty(newImagePath))
            {
                return RedirectToAction(nameof(Images), new { id = id });
            }

            CreateImageDto createImageDto = new CreateImageDto()
            {
                CarId = id,
                Path = newImagePath
            };
            var addImageResponse = await _carService.AddImage(createImageDto);
            if (addImageResponse.Success == false)
                return HandleApiError(addImageResponse.Message, addImageResponse.ValidationErrors);
            return RedirectToAction(nameof(Images), new { id = id });
        }

        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            var imageDeleteResult = await _carService.DeleteImage(id, imageId);
            if (imageDeleteResult.Success == false)
                return HandleApiError(imageDeleteResult.Message, imageDeleteResult.ValidationErrors);
            return RedirectToAction(nameof(Images), new { id = id });
        }

        protected IActionResult HandleApiError(string message, string validationErrors)
        {
            TempData["Error"] = message + " " + validationErrors;
            return RedirectToAction(nameof(Index));
        }
    }
}
