using AutoMapper;
using FribergCarRental.Client.Services.Base;
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
        private List<CarViewModel> _carViewModels = new List<CarViewModel>();
        private List<BookingViewModel> _bookingViewModels = new List<BookingViewModel>();
        private List<UserViewModel> _userViewModels = new List<UserViewModel>();
        private List<ImageViewModel> _imageViewModels = new List<ImageViewModel>();
        private BookingService _bookingService;
        private CarService _carService;
        private ImageService _imageService;
        private ApplicationUserService _applicationUserService;

        public AdminController(IMapper mapper, CarService carRepository,
            BookingService bookingRepository, ImageService imageRepository,
            ApplicationUserService applicationUserRepository, IAuthService authService) : base(authService)
        {
            _mapper = mapper;
            _bookingService = bookingRepository;
            _carService = carRepository;
            _imageService = imageRepository;
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
            _carViewModels = _mapper.Map<List<CarViewModel>>(carsResponse.Data);
            return View(_carViewModels);
        }

        public IActionResult CreateCar()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            var carViewModel = _mapper.Map<CarViewModel>(carResponse.Data);

            if (carViewModel == null)
            {
                return NotFound();
            }
            return View(carViewModel);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Cars));
            }
            return View(carViewModel);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _carService.GetCar(id);
            var carViewModel = _mapper.Map<CarViewModel>(car);
            if (carViewModel == null)
            {
                return NotFound();
            }

            return View(carViewModel);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("DeleteCar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCar(int id)
        {
            var carResponse = await _carService.GetCar(id);
            if (carResponse.Success == true)
            {
                var deleteResponse = await _carService.DeleteCar(carResponse.Data.CarId);
                if (deleteResponse.Success == false)
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Cars));
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Bookings()
        {
            var bookingsResponse = await _bookingService.GetAll();
            if (bookingsResponse.Success == false)
            {
                return NotFound();
            }

            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookingsResponse.Data);
            return View(_bookingViewModels);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);

            if (bookingResponse.Success == false)
            {
                return NotFound();
            }

            var bookingViewModel = _mapper.Map<BookingViewModel>(bookingResponse.Data);
            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Delete/5
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookingConfirmed(int id)
        {

            var bookingResponse = await _bookingService.GetById(id);
            if (bookingResponse.Success == true)
            {
                var deleteResponse = await _bookingService.Delete(bookingResponse.Data.BookingId);
                if (deleteResponse.Success == false)
                {
                    return NotFound();
                }
            }

            return RedirectToAction(nameof(Bookings));
        }


        // GET: Users
        public async Task<IActionResult> Users()
        {
            var usersResponse = await _applicationUserService.GetApplicationUsers();
            if (usersResponse.Success == false)
            {
                return NotFound();
            }
            _userViewModels = _mapper.Map<List<UserViewModel>>(usersResponse.Data);
            return View(_userViewModels);
        }

        // GET: Users/Create
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Password,FirstName,LastName,Address,City,ZipCode,Email")] CreateUserViewModel userViewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(userViewModel.Password))
            {
                var user = _mapper.Map<CreateApplicationUserDto>(userViewModel);
                var addUserResult = await _applicationUserService.AddApplicationUser(user);
                if (addUserResult.Success == false)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Users));
            }
            return View(userViewModel);
        }

        public async Task<IActionResult> EditUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userResult = await _applicationUserService.GetApplicationUser(id);
            var userViewModel = _mapper.Map<UserViewModel>(userResult.Data);

            if (userViewModel == null)
            {
                return NotFound();
            }
            return View(userViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            var userResult = await _applicationUserService.GetApplicationUser(id);
            if (userResult.Success == false)
            {
                return NotFound();
            }
            var result = await _applicationUserService.UpdateApplicationUser(userResult.Data.Id, editedUser);
            if (result.Success == true)
            {
                return RedirectToAction(nameof(Users));
            }

            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError("", error.Description);
            //}
            _mapper.Map(userViewModel, userResult.Data);
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
            var userViewModel = _mapper.Map<UserViewModel>(userResponse.Data);
            if (userViewModel == null)
            {
                return NotFound();
            }
            return View(userViewModel);
        }

        // POST: Users/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUser(string id)
        {
            var deleteUserResult = await _applicationUserService.DeleteApplicationUser(id);
            if (deleteUserResult.Success == false)
            {
                return View(deleteUserResult);
            }
            return RedirectToAction(nameof(Users));
        }


        public async Task<IActionResult> Images(int id)
        {
            var carResponse = await _carService.GetCar(id);
            var imagesResponse = await _imageService.GetImagesByCarId(id);

            var viewModel = _mapper.Map<CarViewModel>(carResponse.Data);
            viewModel.Images = _mapper.Map<List<ImageViewModel>>(imagesResponse.Data);
            return View(viewModel);
        }

        public async Task<IActionResult> AddImage(int id, string newImagePath)
        {
            if (string.IsNullOrEmpty(newImagePath))
            {
                return RedirectToAction(nameof(Images), new { id = id });
            }

            var result = await _carService.GetCar(id);
            var image = new ImageDto()
            {
                CarId = id,
                Path = newImagePath
            };

            await _imageService.AddImage(image);
            return RedirectToAction(nameof(Images), new { id = id });

        }

        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            var imageDeleteResult = await _imageService.DeleteImage(id);
            //if (image != null)
            //{
            //    _imageService.Delete(image);
            //    await _imageService.SaveChangesAsync();
            //}
            return RedirectToAction(nameof(Images), new { id = id });

        }
    }
}
