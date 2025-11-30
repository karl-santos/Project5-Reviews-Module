using System;
using Microsoft.EntityFrameworkCore;
using reviews.Services;
using Xunit;

namespace reviews.Tests
{
    // Simple tests for Reviews Module
    public class ReviewServiceTests
    {
        // Helper method to create in-memory database for testing
        private ReviewDbContext CreateTestDatabase()
        {
            var options = new DbContextOptionsBuilder<ReviewDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ReviewDbContext(options);
        }

        // Test 1: Add a product review successfully
        [Fact]
        public void AddProductReview_ValidInput_SavesReviewToDatabase()
        {
            // Setup
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            // Add a product review
            var result = service.AddProductReview(123, 456, "Great product!", 5);

            // Check it was saved
            Assert.Equal(123, result.ProductID);
            Assert.Equal(456, result.AccountID);
            Assert.Equal("Great product!", result.Comment);
            Assert.Equal(5, result.Rating);
            Assert.Single(service.ProductReviews);
        }

        // Test 2: Invalid rating should throw error
        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void AddProductReview_InvalidRating_ThrowsError(int badRating)
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            var ex = Assert.Throws<ArgumentException>(() =>
                service.AddProductReview(1, 1, "Bad rating", badRating));

            Assert.Equal("Rating must be between 1 and 5", ex.Message);
        }

        // Test 3: Calculate average product rating
        [Fact]
        public void GetAverageProductRating_MultipleReviews_CalculatesCorrectly()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            // Add 3 reviews: 3, 5, 2 -> average = 3.33
            service.AddProductReview(1, 1, "ok", 3);
            service.AddProductReview(1, 2, "good", 5);
            service.AddProductReview(1, 3, "meh", 2);

            var avg = service.GetAverageProductRating(1);

            Assert.Equal(3.33, Math.Round(avg, 2));
        }

        // Test 4: No reviews returns zero average
        [Fact]
        public void GetAverageProductRating_NoReviews_ReturnsZero()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            var avg = service.GetAverageProductRating(999);

            Assert.Equal(0, avg);
        }

        // Test 5: Add a service review successfully
        [Fact]
        public void AddServiceReview_ValidInput_SavesReviewToDatabase()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            var result = service.AddServiceReview(789, 456, "Excellent service!", 5);

            Assert.Equal(789, result.ServiceID);
            Assert.Equal(456, result.AccountID);
            Assert.Equal("Excellent service!", result.Comment);
            Assert.Equal(5, result.Rating);
            Assert.Single(service.ServiceReviews);
        }

        // Test 6: Service review invalid rating throws error
        [Fact]
        public void AddServiceReview_InvalidRating_ThrowsError()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            var ex = Assert.Throws<ArgumentException>(() =>
                service.AddServiceReview(1, 1, "Bad", 10));

            Assert.Contains("Rating must be between 1 and 5", ex.Message);
        }

        // Test 7: Calculate average service rating
        [Fact]
        public void GetAverageServiceRating_MultipleReviews_CalculatesCorrectly()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            service.AddServiceReview(1, 1, "ok", 4);
            service.AddServiceReview(1, 2, "great", 5);

            var avg = service.GetAverageServiceRating(1);

            Assert.Equal(4.5, avg);
        }

        // Test 8: Product and service reviews are separate
        [Fact]
        public void ProductAndServiceReviews_StayedSeparate()
        {
            var db = CreateTestDatabase();
            var service = new ReviewService(db);

            service.AddProductReview(1, 1, "product", 5);
            service.AddServiceReview(1, 1, "service", 3);

            Assert.Single(service.ProductReviews);
            Assert.Single(service.ServiceReviews);
            Assert.Equal(5, service.ProductReviews[0].Rating);
            Assert.Equal(3, service.ServiceReviews[0].Rating);
        }
    }
}
