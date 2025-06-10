using AutoMapper;
using FribergCarRental.Classes;
using FribergCarRental.Data;
using FribergCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRental.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IMapper _mapper;
        private List<BookingViewModel> _bookingViewModels = new();
        private BookingRepository _bookingRepository;
        private CarRepository _carRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public BookingController(IMapper mapper, UserManager<ApplicationUser> userManager,
            BookingRepository bookingRepository, CarRepository carRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _bookingRepository.GetAllBookingsWithCarByUserIdAsync(user.Id);
            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookings);
            return View(_bookingViewModels);
        }

        // GET: BookingViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _bookingRepository.GetBookingWithCarByIdAsync(id);
            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);

            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Cars = new SelectList(await _carRepository.GetAllAsync(), "CarId", "DisplayBrandModel");
            return View();
        }

        // POST: BookingViewModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CarId,RentStartDate,RentEndDate")] BookingViewModel bookingViewModel)
        {
            var car = await _carRepository.GetByIdAsync(bookingViewModel.CarId);
            bookingViewModel.Car = car;
            var user = await _userManager.GetUserAsync(User);
            bookingViewModel.ApplicationUser = user;

            _bookingViewModels = _mapper.Map<List<BookingViewModel>>(await _bookingRepository.GetAllBookingsByCarIdAsync(bookingViewModel.CarId));
            foreach (var oldBooking in _bookingViewModels)
            {
                if (BookingIsOverlapping(bookingViewModel, oldBooking))
                {
                    ModelState.AddModelError(string.Empty, "This car is already booked for the selected dates.");
                    ViewBag.Cars = new SelectList(await _carRepository.GetAllAsync(), "CarId", "DisplayBrandModel");
                    return View(bookingViewModel);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(await _carRepository.GetAllAsync(), "CarId", "DisplayBrandModel");
                return View(bookingViewModel);
            }

            var booking = _mapper.Map<Booking>(bookingViewModel);
            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();
            TempData["SuccessfulBooking"] = $"Booking created successfully! {bookingViewModel.Car.DisplayBrandModel} " +
                $"reserved from {bookingViewModel.RentStartDate} to {bookingViewModel.RentEndDate}.";
            return RedirectToAction(nameof(Index));
        }

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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _bookingRepository.GetBookingWithCarByIdAsync(id);
            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
            ViewBag.Car = booking.Car.DisplayBrandModel;

            if (bookingViewModel == null)
            {
                return NotFound();
            }
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
                try
                {
                    var booking = _mapper.Map<Booking>(bookingViewModel);
                    _bookingRepository.Update(booking);
                    await _bookingRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _bookingRepository.ExistsAsync(bookingViewModel.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _bookingRepository.GetBookingWithCarByIdAsync(id);
            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
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
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (booking.RentStartDate < today)
            {
                ModelState.AddModelError(string.Empty, "You cannot delete a booking that has already started.");
                return View(_mapper.Map<BookingViewModel>(booking));
            }
            _bookingRepository.Delete(booking);
            await _bookingRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
