using System;
using System.Collections.Generic;
using System.Linq;
using reviews.Models;

namespace reviews.Services
{
    public class ReviewService
    {
        private readonly IReviewStore _store;

        // Expose the lists 
        public List<ProductReview> ProductReviews => _store.ProductReviews;
        public List<RentalReview> RentalReviews => _store.RentalReviews;
        public List<TeamReview> TeamReviews => _store.TeamReviews;

        // Used by tests 
        public ReviewService() : this(new InMemoryReviewStore())
        {
        }

        // Used by Program.cs
        public ReviewService(IReviewStore store)
        {
            _store = store;
        }

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

            _store.ProductReviews.Add(review);
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

            _store.RentalReviews.Add(review);
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

            _store.TeamReviews.Add(review);
            return review;
        }

        // Get average rating for a product
        public double GetAverageProductRating(string productId)
        {
            var reviews = _store.ProductReviews
                .Where(r => r.ProductID == productId)
                .ToList();

            if (reviews.Count == 0)
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}
