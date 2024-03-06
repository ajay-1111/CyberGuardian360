using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CyberGuardian360.Models.EFDBContext
{
    public class UserRegistration : IdentityUser
    {

        [StringLength(100)]
        public string GivenName { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
