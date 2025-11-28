using System.Collections.Generic;
using reviews.Models;

namespace reviews.Services
{
    //storage interface so we can swap in-memory with database later.
    public interface IReviewStore
    {
        // Product reviews
        ProductReview SaveProductReview(ProductReview review);
        List<ProductReview> GetProductReviews(string productId);

        // Rental reviews .... soon to be removed 1 day
        RentalReview SaveRentalReview(RentalReview review);
        List<RentalReview> GetRentalReviews(string rentalId);

        // Team reviews
        TeamReview SaveTeamReview(TeamReview review);
        List<TeamReview> GetTeamReviews(string teamId);


        List<ProductReview> ProductReviews { get; }
        List<RentalReview> RentalReviews { get; }
        List<TeamReview> TeamReviews { get; }
    }
}
