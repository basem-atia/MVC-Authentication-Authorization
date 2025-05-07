using Authentication_Authorization.MVC.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication_Authorization.MVC.Data.Context
{
    public class UsersContext : IdentityDbContext<UserModel>
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserModel>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.Role).HasMaxLength(255);
            });
        }
    }
}
