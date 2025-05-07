using System.ComponentModel.DataAnnotations;

namespace Authentication_Authoriztion.MVC.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string userEmail { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
    }
}
