using FribergCarRental.Services.Base;

namespace FribergCarRental.Client.Services.Base
{
    public class ImageService : BaseHttpService
    {
        private readonly IClient _client;

        public ImageService(IClient client, IHttpContextAccessor httpContextAccessor)
            : base(client, httpContextAccessor)
        {
            _client = client;
        }

        public async Task<Response<List<ImageDto>>> GetImagesByCarId(int carId)
        {
            Response<List<ImageDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BycarAsync(carId);
                response = new Response<List<ImageDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<List<ImageDto>>(aex);
            }

            return response;
        }

        public async Task<Response<ImageDto>> GetImage(int id)
        {
            Response<ImageDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.ImageGETAsync(id);
                response = new Response<ImageDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<ImageDto>(aex);
            }

            return response;
        }

        public async Task<Response<ImageDto>> AddImage(ImageDto image)
        {
            Response<ImageDto> response;

            try
            {
                await GetBearerToken();
                var result = await _client.ImagePOSTAsync(image);
                response = new Response<ImageDto>
                {
                    Data = result,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<ImageDto>(aex);
            }

            return response;
        }

        public async Task<Response<int>> DeleteImage(int id)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.ImageDELETEAsync(id);
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

