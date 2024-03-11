using CyberGuardian360.Models.EFDBContext;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CyberGuardian360.DBContext
{
    /// <summary>
    /// The entity framework DB context class.
    /// </summary>
    public class CyberGuardian360DbContext : IdentityDbContext<UserRegistration>
    {
        private readonly IConfiguration configuration;

        public CyberGuardian360DbContext(DbContextOptions<CyberGuardian360DbContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<UserRegistration> AspNetUsers { get; set; } = null!;

        public virtual DbSet<CSProducts> CSProducts { get; set; } = null!;

        public virtual DbSet<UserCartInfo> CSUserCartInfo { get; set; } = null!;

        public virtual DbSet<UserOrder> CSUserOrders { get; set; } = null!;

        public virtual DbSet<UserOrderItem> CSUserOrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
