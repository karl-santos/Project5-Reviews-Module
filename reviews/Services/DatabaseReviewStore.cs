using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using reviews.Models;

namespace reviews.Services
{
    // Database context for reviews
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
        {
        }

        // Tables in the database
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<RentalReview> RentalReviews { get; set; }
        public DbSet<TeamReview> TeamReviews { get; set; }
    }

    // Database implementation of review store
    public class DatabaseReviewStore : IReviewStore
    {
        private readonly ReviewDbContext _context;

        public DatabaseReviewStore(ReviewDbContext context)
        {
            _context = context;
        }

        // Get all product reviews as a list
        public List<ProductReview> ProductReviews => _context.ProductReviews.ToList();

        // Get all rental reviews as a list
        public List<RentalReview> RentalReviews => _context.RentalReviews.ToList();

        // Get all team reviews as a list
        public List<TeamReview> TeamReviews => _context.TeamReviews.ToList();

        // Save a product review to database
        public ProductReview SaveProductReview(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        // Get product reviews for a specific product
        public List<ProductReview> GetProductReviews(string productId)
        {
            return _context.ProductReviews
                .Where(r => r.ProductID == productId)
                .ToList();
        }

        // Save a rental review to database
        public RentalReview SaveRentalReview(RentalReview review)
        {
            _context.RentalReviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        // Get rental reviews for a specific rental
        public List<RentalReview> GetRentalReviews(string rentalId)
        {
            return _context.RentalReviews
                .Where(r => r.RentalID == rentalId)
                .ToList();
        }

        // Save a team review to database
        public TeamReview SaveTeamReview(TeamReview review)
        {
            _context.TeamReviews.Add(review);
            _context.SaveChanges();
            return review;
        }

        // Get team reviews for a specific team
        public List<TeamReview> GetTeamReviews(string teamId)
        {
            return _context.TeamReviews
                .Where(r => r.TeamID == teamId)
                .ToList();
        }
    }
}
