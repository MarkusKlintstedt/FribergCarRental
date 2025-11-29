using AutoMapper;
using FribergCarRental.Models;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FribergCarRental.Controllers
{
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
            var userId = User.FindFirst("uid")?.Value;
            if (userId == null)
            {
                return NotFound("User not found.");
            }
            var bookingsResponse = await _bookingService.GetAllWithCarByUser(userId);
            if (bookingsResponse.Success == false)
                return HandleApiError(bookingsResponse.Message, bookingsResponse.ValidationErrors);

            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookingsResponse.Data);
            return View(_bookingViewModels);
        }

        // GET: BookingViewModels/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);
            if (bookingResponse.Success == false)
                return HandleApiError(bookingResponse.Message, bookingResponse.ValidationErrors);

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
                return HandleApiError(carsResponse.Message, carsResponse.ValidationErrors);

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

            var userId = User.FindFirst("uid")?.Value;

            var createBookingDto = new CreateBookingDto
            {
                CarId = booking.CarId,
                UserId = userId,
                RentStartDate = booking.RentStartDate,
                RentEndDate = booking.RentEndDate
            };

            var response = await _bookingService.Create(createBookingDto);
            if (response.Success == false)
                return HandleApiError(response.Message, response.ValidationErrors);

            TempData["SuccessfulBooking"] =
                $"Booking created successfully!";

            return RedirectToAction(nameof(Index));
        }

        // GET: BookingViewModels/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);
            if (bookingResponse.Success == false)
                return HandleApiError(bookingResponse.Message, bookingResponse.ValidationErrors);

            var bookingViewModel = _mapper.Map<BookingViewModel>(bookingResponse.Data);
            ViewBag.Car = bookingResponse.Data.Car.DisplayBrandModel;
            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,RentStartDate,RentEndDate,CarId")] BookingViewModel bookingViewModel)
        {
            if (id != bookingViewModel.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var booking = _mapper.Map<EditBookingDto>(bookingViewModel);
                booking.UserId = User.FindFirst("uid")?.Value;
                var updateResponse = await _bookingService.Update(id, booking);
                if (updateResponse.Success == false)
                    return HandleApiError(updateResponse.Message, updateResponse.ValidationErrors);

                return RedirectToAction(nameof(Index));
            }
            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var bookingResponse = await _bookingService.GetById(id);
            if (bookingResponse.Success == false)
                return HandleApiError(bookingResponse.Message, bookingResponse.ValidationErrors);

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
            var deleteResult = await _bookingService.Delete(id);
            if (deleteResult.Success == false)
                return HandleApiError(deleteResult.Message, deleteResult.ValidationErrors);

            return RedirectToAction(nameof(Index));
        }

        protected IActionResult HandleApiError(string message, string validationErrors)
        {
            TempData["Error"] = message + " " + validationErrors;
            return RedirectToAction(nameof(Index));
        }

    }
}
