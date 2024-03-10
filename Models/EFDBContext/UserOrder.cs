namespace CyberGuardian360.Models.EFDBContext
{
    public class UserOrder
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
