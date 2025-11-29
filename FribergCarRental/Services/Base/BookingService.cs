using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class BookingService : BaseHttpService
    {
        private readonly IClient _client;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingService(IHttpContextAccessor httpContextAccessor, IClient client)
            : base(client, httpContextAccessor)
        {
            _client = client;
            _contextAccessor = httpContextAccessor;
        }

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

        public async Task<Response<int>> Update(int id, EditBookingDto booking)
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
            catch (ApiException apiEx)
            {
                response = ConvertApiExceptions<int>(apiEx);
            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }

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

        public async Task<Response<List<BookingDto>>> GetAllWithCarByUser(string userId)
        {
            Response<List<BookingDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AllByUserIdAsync(userId);
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
                throw;
            }

            return response;
        }
    }
}

