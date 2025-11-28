using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class CarService : BaseHttpService
    {
        private readonly IClient _client;
        public CarService(IClient client, IHttpContextAccessor httpContextAccessor) : base(client, httpContextAccessor)
        {
            _client = client;
        }

        public async Task<Response<List<CarDto>>> GetCars()
        {
            Response<List<CarDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.CarAllAsync();
                response = new Response<List<CarDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<List<CarDto>>(aex);
            }
            return response;
        }

        public async Task<Response<CarDto>> GetCar(int id)
        {
            Response<CarDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.CarGETAsync(id);
                response = new Response<CarDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<CarDto>(aex);
            }

            return response;
        }

        public async Task<Response<CarDto>> AddCar(CarDto car)
        {
            Response<CarDto> response;

            try
            {
                await GetBearerToken();
                var result = await _client.CarPOSTAsync(car);
                response = new Response<CarDto>
                {
                    Data = result,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<CarDto>(aex);
            }

            return response;
        }

        public async Task<Response<int>> UpdateCar(int id, CarDto car)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.CarPUTAsync(id, car);
                response = new Response<int>
                {
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<int>(aex);
            }

            return response;
        }

        public async Task<Response<int>> DeleteCar(int id)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.CarDELETEAsync(id);
                response.Success = true;
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<int>(aex);
            }

            return response;
        }
    }
}
