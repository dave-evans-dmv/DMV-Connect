using System.ComponentModel.DataAnnotations;

namespace DMVConnect.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Passwords must be between 6 and 25 characters")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
