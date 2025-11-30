using reviews.Models;

namespace reviews.Services
{
    // Review service - saves reviews to MySQL database
    public class ReviewService
    {
        private readonly ReviewDbContext _db;

        public ReviewService(ReviewDbContext db)
        {
            _db = db;
        }

        // Get all product reviews (from database)
        public List<ProductReview> ProductReviews => _db.ProductReviews.ToList();
        
        // Get all service reviews (from database)
        public List<ServiceReview> ServiceReviews => _db.ServiceReviews.ToList();

        // Add a product review (saves to database)
        public ProductReview AddProductReview(int productId, int accountId, string comment, int rating)
        {
            // Validate rating is 1-5
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            // Create new product review
            var review = new ProductReview
            {
                ProductID = productId,
                AccountID = accountId,
                Comment = comment,
                Rating = rating
                // CreatedAt is auto-set by database
            };

            // Save to database
            _db.ProductReviews.Add(review);
            _db.SaveChanges();
            
            return review;
        }

        // Add a service review (saves to database)
        public ServiceReview AddServiceReview(int serviceId, int accountId, string comment, int rating)
        {
            // Validate rating is 1-5
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            // Create new service review
            var review = new ServiceReview
            {
                ServiceID = serviceId,
                AccountID = accountId,
                Comment = comment,
                Rating = rating
                // CreatedAt is auto-set by database
            };

            // Save to database
            _db.ServiceReviews.Add(review);
            _db.SaveChanges();
            
            return review;
        }

        // Get average rating for a product (from database)
        public double GetAverageProductRating(int productId)
        {
            var reviews = _db.ProductReviews
                .Where(r => r.ProductID == productId && r.Rating.HasValue)
                .ToList();

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => r.Rating.Value);
        }

        // Get average rating for a service (from database)
        public double GetAverageServiceRating(int serviceId)
        {
            var reviews = _db.ServiceReviews
                .Where(r => r.ServiceID == serviceId && r.Rating.HasValue)
                .ToList();

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => r.Rating.Value);
        }
    }
}
