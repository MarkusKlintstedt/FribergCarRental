using FribergCarRental.Data;
using FribergCarRental.Middleware;
using FribergCarRental.Services.Authentication;
using FribergCarRental.Services.Base;

namespace FribergCarRental
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7264/") });
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(ClientMappingProfile));
            builder.Services.AddScoped<FribergCarRental.Services.Base.IClient, FribergCarRental.Services.Base.Client>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<CarService>();
            builder.Services.AddScoped<BookingService>();
            builder.Services.AddScoped<ApplicationUserService>();
            builder.Services.AddRazorPages();
            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMiddleware<JwtCookieAuthenticationMiddleware>();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }


}
