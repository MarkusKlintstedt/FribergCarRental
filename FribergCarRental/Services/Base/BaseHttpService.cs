using System.Net.Http.Headers;
using Blazored.LocalStorage;
using FribergCarRental.Client.Services.Base;

namespace FribergCarRental.Services.Base
{
    public class BaseHttpService
    {
        private readonly ILocalStorageService localStorage;
        private readonly IClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseHttpService(ILocalStorageService localStorage, IClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.localStorage = localStorage;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        //public BaseHttpService(ILocalStorageService localStorage, Client.Services.Base.IClient client1)
        //{
        //    this.localStorage = localStorage;
        //}

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
            string token = null;

            try
            {
                token = httpContextAccessor?.HttpContext?.Request?.Cookies?["accessToken"];
            }
            catch
            {
                token = null;
            }

            //if (!string.IsNullOrEmpty(token) && client != null)
            //{
            //    client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            //}

            //var token = await localStorage.GetItemAsync<string>("accessToken");
            if (client?.HttpClient == null)
            {
                // Logga här om du vill
                return;
            }

            if (token != null)
            {
                client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
        }

    }
}
