using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FribergCarRental.Controllers
{
    [Authorize]
    public class BookingController : BaseController
    {
        private readonly IMapper _mapper;
        private List<BookingViewModel> _bookingViewModels = new();
        private BookingService _bookingService;
        private CarService _carService;
        private IAuthService _authenticationService;
        private ApplicationUserService _applicationUserService;


        public BookingController(IMapper mapper, BookingService bookingService, CarService carService, IAuthService authenticationService, ApplicationUserService applcationUserService) : base(authenticationService)
        {
            _mapper = mapper;
            _bookingService = bookingService;
            _carService = carService;
            _authenticationService = authenticationService;
            _applicationUserService = applcationUserService;
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Index()
        {
            var userId = await _authenticationService.GetUserId();
            //var user = await _userManager.GetUserAsync(User);
            if (userId == null)
            {
                return NotFound("User not found.");
            }
            var bookingsResponse = await _bookingService.GetAllWithCarByUser(userId);
            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookingsResponse.Data);
            return View(_bookingViewModels);
        }

        // GET: BookingViewModels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var bookingResponse = await _bookingService.GetBookingWithCar(id);
            var bookingViewModel = _mapper.Map<BookingViewModel>(bookingResponse.Data);

            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Create
        public async Task<IActionResult> Create()
        {
            var carsResponse = await _carService.GetCars();
            if (carsResponse.Success == false)
            {
                return NotFound("No cars in list");
            }

            ViewBag.Cars = new SelectList(carsResponse.Data, "CarId", "DisplayBrandModel");
            return View();
        }

        // POST: BookingViewModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel booking)
        {
            if (!ModelState.IsValid)
                return View(booking);

            var userId = await _authenticationService.GetUserId();

            var createBookingDto = new CreateBookingDto
            {
                CarId = booking.CarId,
                UserId = userId,
                RentStartDate = booking.RentStartDate,
                RentEndDate = booking.RentEndDate
            };

            var response = await _bookingService.Create(createBookingDto);

            if (!response.Success)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(booking);
            }

            TempData["SuccessfulBooking"] =
                $"Booking created successfully!";

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Create([Bind("BookingId,CarId,RentStartDate,RentEndDate")] BookingViewModel bookingViewModel)
        //{
        //    var carResponse = await _carService.GetCar(bookingViewModel.CarId);
        //    if (carResponse.Success == false)
        //    {
        //        ModelState.AddModelError(string.Empty, "Selected car does not exist.");
        //        var carsResponse = await _carService.GetCars();
        //        ViewBag.Cars = new SelectList(carsResponse.Data, "CarId", "DisplayBrandModel");
        //        return View(bookingViewModel);
        //    }
        //    bookingViewModel.Car = _mapper.Map<CarViewModel>(carResponse.Data);

        //    var userId = await _authenticationService.GetUserId();
        //    var userResponse = await _applicationUserService.GetApplicationUser(userId);
        //    var tempUser = _mapper.Map<UserViewModel>(userResponse.Data);
        //    bookingViewModel.ApplicationUser = tempUser;

        //    var bookingsResponse = await _bookingService.GetAllByCar(bookingViewModel.CarId);
        //    _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookingsResponse.Data);
        //    foreach (var oldBooking in _bookingViewModels)
        //    {
        //        if (BookingIsOverlapping(bookingViewModel, oldBooking))
        //        {
        //            ModelState.AddModelError(string.Empty, "This car is already booked for the selected dates.");
        //            var carsResponse = await _carService.GetCars();
        //            ViewBag.Cars = new SelectList(carsResponse.Data, "CarId", "DisplayBrandModel");
        //            return View(bookingViewModel);
        //        }
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var carsResponse = await _carService.GetCars();
        //        ViewBag.Cars = new SelectList(carsResponse.Data, "CarId", "DisplayBrandModel");
        //        return View(bookingViewModel);
        //    }

        //    var createBookingDto = _mapper.Map<BookingDto>(bookingViewModel);
        //    await _bookingService.Create(createBookingDto);
        //    TempData["SuccessfulBooking"] = $"Booking created successfully! {bookingViewModel.Car.Brand} {bookingViewModel.Car.ModelName} " +
        //        $"reserved from {bookingViewModel.RentStartDate} to {bookingViewModel.RentEndDate}.";
        //    return RedirectToAction(nameof(Index));
        //}

        private bool BookingIsOverlapping(BookingViewModel newBooking, BookingViewModel existingBooking)
        {
            if (newBooking.CarId == existingBooking.CarId &&
                  newBooking.RentStartDate <= existingBooking.RentEndDate &&
                  newBooking.RentStartDate >= existingBooking.RentStartDate)
            {
                return true;
            }
            if (newBooking.CarId == existingBooking.CarId &&
                   newBooking.RentEndDate >= existingBooking.RentStartDate &&
                   newBooking.RentEndDate <= existingBooking.RentEndDate)
            {
                return true;
            }
            return false;
        }


        // GET: BookingViewModels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bookingResponse = await _bookingService.GetBookingWithCar(id);
            if (bookingResponse.Success == false)
            {
                return NotFound();
            }
            var bookingViewModel = _mapper.Map<BookingViewModel>(bookingResponse.Data);
            ViewBag.Car = bookingResponse.Data.Car.DisplayBrandModel;
            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RentStartDate,RentEndDate")] BookingViewModel bookingViewModel)
        {
            if (id != bookingViewModel.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var booking = _mapper.Map<BookingDto>(bookingViewModel);
                var updateResponse = await _bookingService.Update(id, booking);
                if (updateResponse.Success == false)
                {
                    return NotFound(updateResponse.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var bookingResponse = await _bookingService.GetBookingWithCar(id);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);

            if (bookingResponse.Success == false)
            {
                return NotFound();
            }

            if (bookingResponse.Data.RentStartDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(string.Empty, "You cannot delete a booking that has already started.");
                return View(_mapper.Map<BookingViewModel>(bookingResponse));
            }
            var deleteResult = _bookingService.Delete(bookingResponse.Data.BookingId);
            return RedirectToAction(nameof(Index));
        }
    }
}
