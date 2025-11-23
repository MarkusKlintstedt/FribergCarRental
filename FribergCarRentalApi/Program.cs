using System.Text;
using FribergCarRental.Api.Mappings;
using FribergCarRental.Core.Classes;
using FribergCarRental.Core.Services;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace FribergCarRentalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            ////builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            ////builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            ////    .AddRoles<IdentityRole>()
            ////    .AddEntityFrameworkStores<ApplicationDbContext>();



            //        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddRoles<IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<CarRepository>();
            builder.Services.AddScoped<BookingRepository>();
            builder.Services.AddScoped<ApplicationUserRepository>();
            builder.Services.AddScoped<TokenService>();


            builder.Services.AddCors(options =>
            {
                //options.AddPolicy("AllowAll",
                //    b => b.AllowAnyMethod()
                //    .AllowAnyHeader()
                //    .AllowAnyOrigin());
                options.AddPolicy("AllowClient", b =>
        b.WithOrigins("https://localhost:7104")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());

            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine("OnMessageReceived körs...");
                        foreach (var c in context.HttpContext.Request.Cookies)
                        {
                            Console.WriteLine($"Cookie: {c.Key} = {c.Value}");
                        }
                        var token = context.HttpContext.Request.Cookies["jwtToken"];
                        Console.WriteLine("Token hittad i cookie: " + (token != null));
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "FribergCar API"));
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowClient");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}
