using System.Collections.Generic;
using System.Linq;
using reviews.Models;

namespace reviews.Services
{
    public class InMemoryReviewStore : IReviewStore
    {
        public List<ProductReview> ProductReviews { get; } = new();
        public List<RentalReview>  RentalReviews  { get; } = new();
        public List<TeamReview>    TeamReviews    { get; } = new();

        // Product
        public ProductReview SaveProductReview(ProductReview review)
        {
            ProductReviews.Add(review);
            return review;
        }

        public List<ProductReview> GetProductReviews(string productId)
        {
            return ProductReviews
                .Where(r => r.ProductID == productId)
                .ToList();
        }

        // Rental soon to be removed 1 day
        public RentalReview SaveRentalReview(RentalReview review)
        {
            RentalReviews.Add(review);
            return review;
        }

        public List<RentalReview> GetRentalReviews(string rentalId)
        {
            return RentalReviews
                .Where(r => r.RentalID == rentalId)
                .ToList();
        }

        // Team
        public TeamReview SaveTeamReview(TeamReview review)
        {
            TeamReviews.Add(review);
            return review;
        }

        public List<TeamReview> GetTeamReviews(string teamId)
        {
            return TeamReviews
                .Where(r => r.TeamID == teamId)
                .ToList();
        }
    }
}
