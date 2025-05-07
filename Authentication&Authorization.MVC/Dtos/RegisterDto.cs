using System.ComponentModel.DataAnnotations;

namespace Authentication_Authoriztion.MVC.Dtos
{
    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        public string userName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string userEmail { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{3,}$",
         ErrorMessage = "Password must be at least 3 characters long and" +
            " contain at least one lowercase letter, one uppercase letter, " +
            "and one number.")]
        public string password { get; set; } = string.Empty;

    }
}
