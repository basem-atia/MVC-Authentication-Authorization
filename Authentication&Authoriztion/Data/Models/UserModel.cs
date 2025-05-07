using Microsoft.AspNetCore.Identity;

namespace Authentication_Authoriztion.Data.Models
{
    public class UserModel : IdentityUser
    {
        public string? Role { get; set; }

    }
}
