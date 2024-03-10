namespace CyberGuardian360.Models.EFDBContext
{
    public class UserCartInfo
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string UserId { get; set; }

        public double ProductCost { get; set; }

        public int Quantity { get; set; }
    }
}
