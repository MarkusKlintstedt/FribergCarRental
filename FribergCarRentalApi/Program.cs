using System.Security.Claims;
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
            // Temporarily enable PII while debugging (remove in production)
            IdentityModelEventSource.ShowPII = true;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<CarRepository>();
            builder.Services.AddScoped<BookingRepository>();
            builder.Services.AddScoped<ApplicationUserRepository>();
            builder.Services.AddScoped<ImageRepository>();
            builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var rawAuth = context.Request.Headers["Authorization"].FirstOrDefault();
                        Console.WriteLine("JwtBearer OnMessageReceived - raw Authorization header: " + (rawAuth ?? "<null>"));

                        if (!string.IsNullOrEmpty(rawAuth))
                        {
                            var tokenRaw = rawAuth;
                            if (tokenRaw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                tokenRaw = tokenRaw.Substring(7);

                            // Basic trimming
                            tokenRaw = tokenRaw.Trim();

                            // Strip common surrounding quotes
                            tokenRaw = tokenRaw.Trim('\"', '\'');

                            // Remove BOMs, zero-width and common invisible chars
                            tokenRaw = tokenRaw
                                .Replace("\uFEFF", "") // BOM
                                .Replace("\u200B", "") // zero-width space
                                .Replace("\u200C", "")
                                .Replace("\u200D", "")
                                .Replace("\u200E", "")
                                .Replace("\u200F", "");

                            // Remove any control chars (CR/LF already handled by Trim but be safe)
                            tokenRaw = new string(tokenRaw.Where(c => !char.IsControl(c)).ToArray());

                            // URL-decode if the token was accidentally encoded
                            try { tokenRaw = Uri.UnescapeDataString(tokenRaw); } catch { /* ignore */ }

                            context.Token = tokenRaw;
                            Console.WriteLine("JwtBearer OnMessageReceived - normalized token set (length " + tokenRaw.Length + ")");

                            // Diagnostic: inspect header segment and attempt decode to show exact failure data
                            var headerSegment = tokenRaw.Split('.').FirstOrDefault();
                            if (!string.IsNullOrEmpty(headerSegment))
                            {
                                try
                                {
                                    // Try a Base64Url decode to confirm validity
                                    var bytes = Microsoft.IdentityModel.Tokens.Base64UrlEncoder.DecodeBytes(headerSegment);
                                    var decoded = Encoding.UTF8.GetString(bytes);
                                    Console.WriteLine("Jwt header decoded successfully: " + decoded);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Jwt header decode FAILED: " + ex.Message);
                                    Console.WriteLine("Header segment raw value: [" + headerSegment + "]");

                                    // Print character codes to reveal invisible characters
                                    for (int i = 0; i < headerSegment.Length; i++)
                                    {
                                        var ch = headerSegment[i];
                                        Console.WriteLine($"char[{i}] = U+{(int)ch:X4} ('{(char.IsWhiteSpace(ch) ? ' ' : ch)}')");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("JwtBearer OnMessageReceived - no Authorization header");
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = c =>
                    {
                        Console.WriteLine("JWT ERROR: " + c.Exception.ToString());
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
