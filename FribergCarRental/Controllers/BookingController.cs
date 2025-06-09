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
        //private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private List<BookingViewModel> _bookingViewModels = new();
        private BookingRepository _bookingRepository;
        private CarRepository _carRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public BookingController(IMapper mapper, UserManager<ApplicationUser> userManager,
            BookingRepository bookingRepository, CarRepository carRepository)
        {
            //_context = context; ApplicationDbContext context, 
            _mapper = mapper;
            _userManager = userManager;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        // GET: BookingViewModels
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //var bookings = await _context.Bookings.Where(b => b.ApplicationUser == user).Include(b => b.Car).ToListAsync();
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
            //var booking = await _bookingRepository.GetAllWithCarAsync().FirstOrDefault(b => b.BookingId == id);
            //var booking = await _context.Bookings.Include(b => b.Car).FirstOrDefaultAsync(m => m.BookingId == id);
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
            //ViewBag.Cars = new SelectList(_context.Cars, "CarId", "DiaplayBrandModel");
            ViewBag.Cars = new SelectList(await _carRepository.GetAllAsync(), "CarId", "DisplayBrandModel");
            return View();
        }

        // POST: BookingViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CarId,RentStartDate,RentEndDate")] BookingViewModel bookingViewModel)
        {
            //Car car = _context.Cars.Find(bookingViewModel.CarId);
            var car = await _carRepository.GetByIdAsync(bookingViewModel.CarId);
            bookingViewModel.Car = car;
            var user = await _userManager.GetUserAsync(User);
            bookingViewModel.ApplicationUser = user;

            if (ModelState.IsValid)
            {
                var booking = _mapper.Map<Booking>(bookingViewModel);
                await _bookingRepository.AddAsync(booking);
                await _bookingRepository.SaveChangesAsync();
                //_context.Add(booking);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cars = new SelectList(await _carRepository.GetAllAsync(), "CarId", "DisplayBrandModel");
            return View(bookingViewModel);
        }

        // GET: BookingViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _bookingRepository.GetBookingWithCarByIdAsync(id);
            //var booking = _bookingRepository.GetAllWithCar().FirstOrDefault(b => b.BookingId == id);
            //var booking = await _context.Bookings.Include(b => b.Car).FirstOrDefaultAsync(m => m.BookingId == id);
            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
            ViewBag.Car = booking.Car.DisplayBrandModel;

            if (bookingViewModel == null)
            {
                return NotFound();
            }
            return View(bookingViewModel);
        }

        // POST: BookingViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    //_context.Update(booking);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!await BookingViewModelExistsAsync(bookingViewModel.BookingId))
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
            //var booking = _bookingRepository.GetAllWithCar().FirstOrDefault(b => b.BookingId == id);
            //var booking = await _context.Bookings
            //    .FirstOrDefaultAsync(m => m.BookingId == id);
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
            //var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _bookingRepository.Delete(booking);
                //_context.Bookings.Remove(booking);
            }
            await _bookingRepository.SaveChangesAsync();
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private async Task<bool> BookingViewModelExistsAsync(int id)
        //{
        //    return await _bookingRepository.GetByIdAsync(id) != null;
        //    //return _context.Bookings.Any(e => e.BookingId == id);
        //}
    }
}
