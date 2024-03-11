using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CyberGuardian360.Models
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Given Name is mandatory.")]
        [StringLength(100)]
        [DisplayName("Given Name")]
        public string GivenName { get; set; }

        [Required(ErrorMessage = "Surname is mandatory.")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Phone number is mandatory.")]
        [StringLength(100, MinimumLength = 10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is mandatory.")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is mandatory.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
