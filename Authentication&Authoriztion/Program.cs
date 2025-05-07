
using Authentication_Authoriztion.Data.Context;
using Authentication_Authoriztion.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Authentication_Authoriztion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Default
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            #endregion

            #region authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //secret key change to symmetric key req for token
                    var secretKey = builder.Configuration.GetValue<string>("SecretKey")!;
                    var secretKeyInBytes = Encoding.UTF8.GetBytes(secretKey);
                    var key = new SymmetricSecurityKey(secretKeyInBytes);
                    // verify token after login and send another req
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                    };
                });


            #endregion

            #region Database connection
            builder.Services.AddDbContext<UsersDbContext>(
                options =>
                {
                    var connectionString = builder.Configuration.GetConnectionString("Default");
                    options.UseSqlServer(connectionString);
                }
                );
            #endregion

            #region Identity
            builder.Services.AddIdentityCore<UserModel>(options =>
            {
                // validations
                // this can validate for email and password only other can be made 
                // by fluent validation
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<UsersDbContext>();
            #endregion

            #region authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Constants.Policies.ForAdminOnly,
                    builder => builder
                              .RequireClaim(ClaimTypes.Role, "Admin")
                              .RequireClaim(ClaimTypes.NameIdentifier));
                options.AddPolicy(
                    Constants.Policies.ForTestingOnly,
                    builder => builder
                              .RequireClaim(ClaimTypes.Role, "User")
                              .RequireClaim(ClaimTypes.NameIdentifier)
    );
            });
            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            #region authentication
            app.UseAuthentication();
            #endregion

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
