using System.ComponentModel;

namespace CyberGuardian360.Models
{
    public class CSProductsViewModel
    {
        public int Id { get; set; }

        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        public double ProductCost { get; set; }

        [DisplayName("Product Rating")]
        public double ProductRating { get; set; }

        [DisplayName("Product Image")]
        public string? ImageUrl { get; set; }

        [DisplayName("Product Description")]
        public string? ProductDescription { get; set; }
    }
}
