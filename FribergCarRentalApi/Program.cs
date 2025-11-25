using System.Text;
using FribergCarRental.Api.Mappings;
using FribergCarRental.Core.Classes;
using FribergCarRental.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
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

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<CarRepository>();
            builder.Services.AddScoped<BookingRepository>();
            builder.Services.AddScoped<ApplicationUserRepository>();
            builder.Services.AddScoped<ImageRepository>();
            //builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            IdentityModelEventSource.ShowPII = true;

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
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
                options.RequireHttpsMetadata = false; // för lokalt arbete
                options.SaveToken = true;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        Console.WriteLine("JWT ERROR: " + c.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = c =>
                    {
                        Console.WriteLine("JWT VALIDATED for: " + c.Principal.Identity.Name);
                        return Task.CompletedTask;
                    }
                };

            });

            Console.WriteLine("Issuer: " + builder.Configuration["JwtSettings:Issuer"]);
            Console.WriteLine("Audience: " + builder.Configuration["JwtSettings:Audience"]);
            Console.WriteLine("Key: " + builder.Configuration["JwtSettings:Key"]);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "FribergCar API"));
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();


            app.Use(async (context, next) =>
            {
                Console.WriteLine("--- INCOMING REQUEST ---");
                Console.WriteLine("Auth Header: " + context.Request.Headers["Authorization"].FirstOrDefault());
                await next();
            });



            app.MapControllers();


            app.Run();
        }
    }
}
