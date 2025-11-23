using Blazored.LocalStorage;
using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class ApplicationUserService : BaseHttpService
    {
        private readonly IClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationUserService(ILocalStorageService localStorage, IClient client, IHttpContextAccessor httpContextAccessor) : base(localStorage, client, httpContextAccessor)
        {
            _client = client;

        }

        public async Task<Response<List<ApplicationUserDto>>> GetApplicationUsers()
        {
            Response<List<ApplicationUserDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.ApplicationUserAllAsync();
                response = new Response<List<ApplicationUserDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<List<ApplicationUserDto>>(aex);
            }
            return response;
        }

        public async Task<Response<ApplicationUserDto>> GetApplicationUser(string id)
        {
            Response<ApplicationUserDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.ApplicationUserGETAsync(id);
                response = new Response<ApplicationUserDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<ApplicationUserDto>(aex);
            }

            return response;
        }
        public async Task<Response<ApplicationUserDto>> AddApplicationUser(CreateApplicationUserDto user)
        {
            Response<ApplicationUserDto> response;

            try
            {
                await GetBearerToken();
                var result = await _client.ApplicationUserPOSTAsync(user);
                response = new Response<ApplicationUserDto>
                {
                    Data = result,
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<ApplicationUserDto>(aex);
            }

            return response;
        }

        public async Task<Response<string>> UpdateApplicationUser(string id, ApplicationUserDto user)
        {
            Response<string> response = new();

            try
            {
                await GetBearerToken();
                await _client.ApplicationUserPUTAsync(id, user);
                response = new Response<string>
                {
                    Success = true
                };
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<string>(aex);
            }

            return response;
        }

        public async Task<Response<string>> DeleteApplicationUser(string id)
        {
            Response<string> response = new();

            try
            {
                await GetBearerToken();
                await _client.ApplicationUserDELETEAsync(id);
                response.Success = true;
            }
            catch (ApiException aex)
            {
                response = ConvertApiExceptions<string>(aex);
            }

            return response;
        }

    }
}
