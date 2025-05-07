using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorization.MVC.Data.Models
{
    public class UserModel : IdentityUser
    {
        public string? Role { get; set; }

    }
}
