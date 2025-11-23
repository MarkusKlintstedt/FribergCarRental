using Blazored.LocalStorage;
using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class BookingService : BaseHttpService
    {
        private readonly IClient _client;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingService(ILocalStorageService localStorage, IHttpContextAccessor httpContextAccessor, IClient client)
            : base(localStorage, client, httpContextAccessor)
        {
            _client = client;
            _contextAccessor = httpContextAccessor;
        }

        // -------------------------
        // Get all bookings
        // -------------------------
        public async Task<Response<List<BookingDto>>> GetAll()
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BookingAllAsync();
                response = new Response<List<BookingDto>>
                {
                    Success = true,
                    Data = data.ToList()
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookingDto>>(ex);
            }

            return response;
        }

        // -------------------------
        // Get booking by id
        // -------------------------
        public async Task<Response<BookingDto>> GetById(int id)
        {
            Response<BookingDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BookingGETAsync(id);
                response = new Response<BookingDto>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookingDto>(ex);
            }

            return response;
        }

        // -------------------------
        // Create booking
        // -------------------------
        public async Task<Response<BookingDto>> Create(CreateBookingDto booking)
        {
            Response<BookingDto> response;

            try
            {
                await GetBearerToken();
                await _client.BookingPOSTAsync(booking);
                response = new Response<BookingDto>
                {
                    Success = true,
                    Data = null
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookingDto>(ex);
            }

            return response;
        }

        // -------------------------
        // Update booking
        // -------------------------
        public async Task<Response<int>> Update(int id, BookingDto booking)
        {
            Response<int> response;

            try
            {
                await GetBearerToken();
                await _client.BookingPUTAsync(id, booking);

                response = new Response<int>
                {
                    Success = true,
                    Data = id
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        // -------------------------
        // Delete booking
        // -------------------------
        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response;

            try
            {
                await GetBearerToken();
                await _client.BookingDELETEAsync(id);

                response = new Response<int>
                {
                    Success = true,
                    Data = id
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        // -------------------------
        // Get all with Car AND User
        // GET: api/booking/allwithcaranduser
        // -------------------------
        public async Task<Response<List<BookingDto>>> GetAllWithCarAndUser()
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AllwithcaranduserAsync();
                response = new Response<List<BookingDto>>
                {
                    Success = true,
                    Data = data.ToList()
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookingDto>>(ex);
            }

            return response;
        }

        // -------------------------
        // Get all with car
        // GET: api/booking/allwithcar
        // -------------------------
        public async Task<Response<List<BookingDto>>> GetAllWithCar()
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AllwithcarAsync();
                response = new Response<List<BookingDto>>
                {
                    Success = true,
                    Data = data.ToList()
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookingDto>>(ex);
            }

            return response;
        }

        // -------------------------
        // Get all with car by user
        // GET: api/booking/allwithcarbyuser/{userId}
        // -------------------------
        public async Task<Response<List<BookingDto>>> GetAllWithCarByUser(string userId)
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AllwithcarbyuserAsync(userId);
                response = new Response<List<BookingDto>>
                {
                    Success = true,
                    Data = data.ToList()
                };
            }
            catch (ApiException apiEx)
            {
                Console.WriteLine("ApiException: " + apiEx.ToString());
                response = ConvertApiExceptions<List<BookingDto>>(apiEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General exception: " + ex.ToString());
                throw; // låt den bubbla upp så vi ser vad som händer
            }
            //catch (ApiException ex)
            //{
            //    response = ConvertApiExceptions<List<BookingDto>>(ex);
            //}

            return response;
        }

        // -------------------------
        // Get booking with car info
        // GET: api/booking/allwithcar/{id}
        // -------------------------
        public async Task<Response<BookingDto>> GetBookingWithCar(int id)
        {
            Response<BookingDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.WithcarAsync(id);
                response = new Response<BookingDto>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookingDto>(ex);
            }

            return response;
        }

        // -------------------------
        // Get all bookings by car
        // GET: api/booking/allbycar/{carId}
        // -------------------------
        public async Task<Response<List<BookingDto>>> GetAllByCar(int carId)
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AllbycarAsync(carId);
                response = new Response<List<BookingDto>>
                {
                    Success = true,
                    Data = data.ToList()
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookingDto>>(ex);
            }

            return response;
        }
    }
}

