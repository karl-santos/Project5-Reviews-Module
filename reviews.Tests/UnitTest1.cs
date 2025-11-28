using System;
using reviews.Services;
using Xunit;

namespace reviews.Tests
{
    public class ReviewServiceTests
    {
        // Product review tests (3 tests)

        [Fact]
        public void AddProductReview_ValidInput_AddsReviewAndReturnsSameObject()
        {
            // happy path: one review gets stored with the right data
            var service = new ReviewService();
            var productId = "P123";
            var userId = "U1";
            var comment = "Great product";
            var rating = 5;

            var result = service.AddProductReview(productId, userId, comment, rating);

            Assert.Single(service.ProductReviews);
            Assert.Equal(productId, result.ProductID);
            Assert.Equal(userId, result.UserID);
            Assert.Equal(comment, result.Comment);
            Assert.Equal(rating, result.Rating);
            Assert.True((DateTime.UtcNow - result.CreatedAt) < TimeSpan.FromSeconds(5));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void AddProductReview_InvalidRating_ThrowsArgumentException(int invalidRating)
        {
            // rating outside 1–5 should not save anything
            var service = new ReviewService();

            var ex = Assert.Throws<ArgumentException>(() =>
                service.AddProductReview("P1", "U1", "Bad rating", invalidRating));

            Assert.Equal("Rating must be between 1 and 5", ex.Message);
            Assert.Empty(service.ProductReviews);
        }

        [Fact]
        public void GetAverageProductRating_NoReviews_ReturnsZero()
        {
            // with no reviews we just expect 0 
            var service = new ReviewService();

            var avg = service.GetAverageProductRating("P1");

            Assert.Equal(0, avg);
        }

         [Fact]
        public void GetAverageProductRating_MultipleReviews_ComputesCorrectAverage()
        {
            //average check: (3 + 5 + 2) / 3 = 3.33
            var service = new ReviewService();
            service.AddProductReview("P1", "U1", "ok", 3);
            service.AddProductReview("P1", "U2", "good", 5);
            service.AddProductReview("P1", "U3", "meh", 2);

            var avg = service.GetAverageProductRating("P1");

            Assert.Equal(3.33, Math.Round(avg, 2));
        }

        [Fact]
        public void GetAverageProductRating_IgnoresOtherProducts()
        {
            // make sure ratings for different product IDs don't get mixed
            var service = new ReviewService();
            service.AddProductReview("P1", "U1", "p1", 5);
            service.AddProductReview("P2", "U2", "p2", 1);

            var avgP1 = service.GetAverageProductRating("P1");
            var avgP2 = service.GetAverageProductRating("P2");

            Assert.Equal(5, avgP1);
            Assert.Equal(1, avgP2);
        }
    }
}
