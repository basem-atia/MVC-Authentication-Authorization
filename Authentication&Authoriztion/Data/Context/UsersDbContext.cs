using Authentication_Authoriztion.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication_Authoriztion.Data.Context
{
    public class UsersDbContext : IdentityDbContext<UserModel>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
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
