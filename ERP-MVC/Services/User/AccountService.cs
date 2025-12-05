using ERP_MVC.Models.Identity;
using ERP_MVC.Models.ViewModels.User;
using System.Net.Http.Json;

namespace ERP_MVC.Services.User
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7052/api/Account/");
        }

        public async Task<bool> Register(UserRegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("Register", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<LoginResult?> Login(UserLoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("Login", model);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResult>();
            }
            else
            {
                var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                if (errorContent != null && errorContent.ContainsKey("errors"))
                {
                    var errors = errorContent["errors"] as IEnumerable<object>;
                    var message = errors != null ? string.Join(", ", errors) : "Login failed";
                    throw new Exception(message);
                }
                else
                {
                    throw new Exception("Login failed");
                }
            }
        }
    }
}
