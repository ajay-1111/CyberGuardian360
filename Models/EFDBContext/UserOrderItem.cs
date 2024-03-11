namespace CyberGuardian360.Models.EFDBContext
{
    public class UserOrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        // Navigation properties
        public UserOrder Order { get; set; }

        public CSProducts Product { get; set; }
    }
}
