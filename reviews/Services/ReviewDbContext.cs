using Microsoft.EntityFrameworkCore;
using reviews.Models;

namespace reviews.Services
{
    // Database context - connects to MySQL and manages ProductReview and ServiceReview tables
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
        {
        }

        // Database tables
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ServiceReview> ServiceReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ProductReview table
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.ToTable("ProductReview");
                entity.HasKey(e => e.ReviewID);
                entity.Property(e => e.ReviewID).ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("current_timestamp()");
            });

            // Configure ServiceReview table
            modelBuilder.Entity<ServiceReview>(entity =>
            {
                entity.ToTable("ServiceReview");
                entity.HasKey(e => e.ReviewID);
                entity.Property(e => e.ReviewID).ValueGeneratedOnAdd();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("current_timestamp()");
            });
        }
    }
}
