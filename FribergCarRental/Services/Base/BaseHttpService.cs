using System.Net.Http.Headers;
using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class BaseHttpService
    {
        private readonly IClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseHttpService(IClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException apiException)
        {
            if (apiException.StatusCode == 400)
            {
                return new Response<Guid>() { Message = "Validation errors have occured.", ValidationErrors = apiException.Response, Success = false };
            }
            if (apiException.StatusCode == 404)
            {
                return new Response<Guid>() { Message = "The requested item could not be found.", Success = false };
            }
            return new Response<Guid>() { Message = "Something went  wrong, please try again...", Success = false };
        }

        protected async Task GetBearerToken()
        {
            string token = httpContextAccessor.HttpContext.Request.Cookies["jwtToken"];
            if (token != null)
            {
                client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
        }

    }
}
