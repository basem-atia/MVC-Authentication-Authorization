using Authentication_Authorization.MVC.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Authentication_Authorization.MVC.Data.Models;

namespace Authentication_Authorization.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            #region Database
            builder.Services.AddDbContext<UsersContext>(
            options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Default");
                options.UseSqlServer(connectionString);
            }
            );

            #endregion
            #region identity
            builder.Services.AddDefaultIdentity<UserModel>(options =>
            {

            }).AddEntityFrameworkStores<UsersContext>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
