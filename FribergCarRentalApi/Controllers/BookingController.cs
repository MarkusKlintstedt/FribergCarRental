using AutoMapper;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Dtos;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BookingController : ControllerBase
    {
        public BookingRepository _bookingRepository { get; set; }
        public CarRepository _carRepository { get; set; }
        public IMapper _mapper { get; set; }
        public ApplicationUserRepository _applicationUserRepository { get; set; }

        public BookingController(BookingRepository bookingRepository, IMapper mapper, CarRepository carRepository, ApplicationUserRepository applicationUserRepository)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _carRepository = carRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetAll()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
            return bookingDtos.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetById(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"No booking with id {id} was found");
            }
            var bookingDto = _mapper.Map<BookingDto>(booking);
            return bookingDto;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var car = await _carRepository.GetByIdAsync(createBookingDto.CarId);
            if (car == null)
                return NotFound("Car not found.");

            if (createBookingDto.RentStartDate >= createBookingDto.RentEndDate)
                return BadRequest("Rent end date must be after start date.");

            if (createBookingDto.RentStartDate < DateOnly.FromDateTime(DateTime.Now))
                return BadRequest("Start date cannot be in the past.");

            var existingBookings = await _bookingRepository.GetAllByCarIdAsync(createBookingDto.CarId);

            bool overlaps = existingBookings.Any(b =>
                createBookingDto.RentStartDate < b.RentEndDate &&
                createBookingDto.RentEndDate > b.RentStartDate);

            if (overlaps)
                return BadRequest("The car is already booked for the selected dates.");

            var booking = new Booking
            {
                CarId = createBookingDto.CarId,
                ApplicationUser = await _applicationUserRepository.GetByIdAsync(createBookingDto.UserId),
                RentStartDate = createBookingDto.RentStartDate,
                RentEndDate = createBookingDto.RentEndDate
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            var bookingDto = _mapper.Map<BookingDto>(booking);

            return Ok(bookingDto);
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<BookingDto>> GetById(int id)
        //{
        //    var booking = await _bookingRepository.GetByIdAsync(id);
        //    if (booking == null)
        //        return NotFound();

        //    return Ok(_mapper.Map<BookingDto>(booking));
        //}

        //[HttpPost]
        //public async Task<ActionResult<BookingDto>> AddBooking([FromBody] BookingDto editBookingDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var booking = _mapper.Map<Booking>(editBookingDto);
        //    await _bookingRepository.AddAsync(booking);
        //    await _bookingRepository.SaveChangesAsync();

        //    var createdBookingDto = _mapper.Map<BookingDto>(booking);
        //    return CreatedAtAction(nameof(GetById), new { id = booking.CarId }, createdBookingDto);
        //}

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateBooking(int id, [FromBody] EditBookingDto editBookingDto)
        {
            if (id != editBookingDto.BookingId)
            {
                return BadRequest("Id mismatch");
            }

            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            _mapper.Map(editBookingDto, existingBooking);
            _bookingRepository.Update(existingBooking);
            await _bookingRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            if (booking.RentStartDate < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest("You cannot delete a booking that has already started.");
            }

            _bookingRepository.Delete(booking);
            await _bookingRepository.SaveChangesAsync();
            return Ok();
        }

        //[HttpGet("allwithcaranduser")]
        //public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookingsWithCarAndUser()
        //{
        //    var bookings = await _bookingRepository.GetAllAsync();
        //    var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
        //    return Ok(bookingDtos);
        //}

        //[HttpGet("allwithcar")]
        //public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllWithCar()
        //{
        //    var bookings = await _bookingRepository.GetAllWithCarAsync();
        //    var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
        //    return Ok(bookingDtos);
        //}

        [HttpGet("AllByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookingsByUser(string userId)
        {
            var bookings = await _bookingRepository.GetAllByUserIdAsync(userId);
            var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
            return Ok(bookingDtos);
        }

        //[HttpGet("withcar/{id}")]
        //public async Task<ActionResult<BookingDto>> GetBookingWithCarById(int id)
        //{
        //    var booking = await _bookingRepository.GetByIdAsync(id);
        //    if (booking == null)
        //        return NotFound();

        //    var editBookingDto = _mapper.Map<BookingDto>(booking);
        //    return Ok(editBookingDto);
        //}

        [HttpGet("AllByCarId/{carId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookingsByCar(int carId)
        {
            var bookings = await _bookingRepository.GetAllByCarIdAsync(carId);
            var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
            return Ok(bookingDtos);
        }
    }

}

