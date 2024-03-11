using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CyberGuardian360.Models.EFDBContext
{
    public class CSProducts
    {
        public enum CSCategories
        {
            [Description("Antivirus Software")]
            AntivirusSoftware = 1,

            [Description("Firewall Solutions")]
            FirewallSolutions = 2,

            [Description("Data Encryption Tools")]
            DataEncryptionTools = 3,
        }
        public int Id { get; set; }

        [DisplayName("Product Name")]
        [Required]
        public string ProductName { get; set; }

        [Required]
        [DisplayName("Product Cost")]
        public double ProductCost { get; set; }

        [Required]
        [DisplayName("Product Rating")]
        public double ProductRating { get; set; }

        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }

        [DisplayName("Product Image")]
        public string? ImageUrl { get; set; }

        [DisplayName("Product Category")]
        [Required]
        public CSCategories ProductCategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
