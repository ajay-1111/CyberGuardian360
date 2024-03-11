using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CyberGuardian360.Models
{
    public class Profile
    {
        [StringLength(100)]
        [DisplayName("Given Name")]
        [Required]
        public string GivenName { get; set; }

        [StringLength(100)]
        [Required]
        public string Surname { get; set; }

        [StringLength(100)]
        [Required]
        public string Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }
    }
}
