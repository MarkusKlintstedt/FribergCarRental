using AutoMapper;
using FribergCarRental.Classes;
using FribergCarRental.Data;
using FribergCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMapper _mapper;
        private List<CarViewModel> _carViewModels = new List<CarViewModel>();
        private List<BookingViewModel> _bookingViewModels = new List<BookingViewModel>();
        private List<UserViewModel> _userViewModels = new List<UserViewModel>();
        private List<ImageViewModel> _imageViewModels = new List<ImageViewModel>();
        private BookingRepository _bookingRepository;
        private CarRepository _carRepository;
        private ImageRepository _imageRepository;
        private ApplicationUserRepository _applicationUserRepository;

        public AdminController(IMapper mapper, CarRepository carRepository,
            BookingRepository bookingRepository, ImageRepository imageRepository, ApplicationUserRepository applicationUserRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _imageRepository = imageRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Cars()
        {
            var cars = await _carRepository.GetAllAsync();
            _carViewModels = _mapper.Map<List<CarViewModel>>(cars);
            foreach (var car in _carViewModels)
            {
                car.Images = _mapper.Map<List<ImageViewModel>>(await _imageRepository.GetAllImagesByCarIdAsync(car.CarId));
            }
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
                var car = _mapper.Map<Car>(carViewModel);
                await _carRepository.AddAsync(car);
                await _carRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Cars));
            }
            return View(carViewModel);
        }

        public async Task<IActionResult> EditCar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carRepository.GetByIdAsync(id);
            var carViewModel = _mapper.Map<CarViewModel>(car);

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
                try
                {
                    var car = _mapper.Map<Car>(carViewModel);
                    _carRepository.Update(car);
                    await _carRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _carRepository.ExistsAsync(carViewModel.CarId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Cars));
            }
            return View(carViewModel);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> DeleteCar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carRepository.GetByIdAsync(id);
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
            var car = await _carRepository.GetByIdAsync(id);
            if (car != null)
            {
                _carRepository.Delete(car);
            }
            await _carRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Cars));
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Bookings()
        {
            var bookings = await _bookingRepository.GetAllWithCarAndUserAsync();
            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookings);
            return View(_bookingViewModels);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Delete/5
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookingConfirmed(int? id)
        {

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking != null)
            {
                _bookingRepository.Delete(booking);
            }

            await _bookingRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Bookings));
        }


        // GET: Users
        public async Task<IActionResult> Users()
        {
            var users = await _applicationUserRepository.GetAllAsync();
            _userViewModels = _mapper.Map<List<UserViewModel>>(users);
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
        public async Task<IActionResult> CreateUser([Bind("NewPassword,UserName,FirstName,LastName,Address,City,ZipCode,PhoneNumber")] UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.UserName,
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    Address = userViewModel.Address,
                    City = userViewModel.City,
                    ZipCode = userViewModel.ZipCode,
                    PhoneNumber = userViewModel.PhoneNumber
                };
                await _applicationUserRepository.CreateUserAsync(user, userViewModel.NewPassword);
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

            var user = await _applicationUserRepository.GetByIdAsync(id);
            var userViewModel = _mapper.Map<UserViewModel>(user);

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
        public async Task<IActionResult> EditUser(String id, [Bind("Id,UserName,FirstName,LastName,Address,City,ZipCode,PhoneNumber")] UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            _mapper.Map(userViewModel, user);
            var result = await _applicationUserRepository.UpdateUserAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userViewModel);
        }


        // GET: User/Delete/5
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _applicationUserRepository.GetByIdAsync(id);
            var userViewModel = _mapper.Map<UserViewModel>(user);
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
            var user = await _applicationUserRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _applicationUserRepository.DeleteUserAsync(user);
            }

            return RedirectToAction(nameof(Users));
        }


        public async Task<IActionResult> Images(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            var images = await _imageRepository.GetAllImagesByCarIdAsync(id);

            var viewModel = _mapper.Map<CarViewModel>(car);
            viewModel.Images = _mapper.Map<List<ImageViewModel>>(images);
            return View(viewModel);
        }

        public async Task<IActionResult> AddImage(int id, string newImagePath)
        {
            if (string.IsNullOrEmpty(newImagePath))
            {
                return RedirectToAction(nameof(Images), new { id = id });
            }
            var image = new Image()
            {
                CarId = id,
                Car = await _carRepository.GetByIdAsync(id),
                Path = newImagePath
            };

            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Images), new { id = id });

        }

        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            var image = await _imageRepository.GetByIdAsync(imageId);
            if (image != null)
            {
                _imageRepository.Delete(image);
                await _imageRepository.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Images), new { id = id });

        }
    }
}
