using reviews.Models;

namespace reviews.Services
{
    public class ReviewService
    {


        public List<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public List<RentalReview> RentalReviews { get; set; } = new List<RentalReview>();
        public List<TeamReview> TeamReviews { get; set; } = new List<TeamReview>();



        // Add a product review
        public ProductReview AddProductReview(string productId, string userId, string comment, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new ProductReview
            {
                ProductID = productId,
                UserID = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };


            // save to in-memory list for demo purposes
            ProductReviews.Add(review);

            // Save to database here
            return review;
        }

        // Add a rental review
        public RentalReview AddRentalReview(string rentalId, string userId, string comment, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new RentalReview
            {
                RentalID = rentalId,
                UserID = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };


            // save to in-memory list for demo purposes
            RentalReviews.Add(review);
            // Save to database here
            return review;
        }

        // Add a team review
        public TeamReview AddTeamReview(string teamId, string userId, string comment, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new TeamReview
            {
                TeamID = teamId,
                UserID = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };


            TeamReviews.Add(review);
            // Save to database here
            return review;
        }

        // Get average rating for a product
        public double GetAverageProductRating(string productId)
        {
            var reviews = ProductReviews.Where(r => r.ProductID == productId).ToList();

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}
