using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using reviews.Controllers;
using reviews.Models;
using reviews.Services;
using Xunit;

namespace reviews.Tests
{
    /// Integration Tests - Test the full API endpoints with real HTTP requests
    /// These tests verify that the entire application works together correctly

    [Collection("Sequential")]
    public class ReviewApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _testDbName;

        public ReviewApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Create a database for this test run
            _testDbName = "TestDb_" + Guid.NewGuid().ToString();
            
            // Create a test server with in-memory database
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ReviewDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in memory database for testing
                    // here I am using instance variable so each test gets the same database within its HTTP requests
                    services.AddDbContext<ReviewDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(_testDbName);
                    }, ServiceLifetime.Scoped);
                });
            });

            _client = _factory.CreateClient();
        }

        // PRODUCT REVIEW TESTS 

        [Fact]
        public async Task PostProductReview_ValidData_ReturnsCreatedReview()
        {
            // Arrange - preparing the review data
            var reviewRequest = new ProductReviewRequest
            {
                ProductID = 1,
                AccountID = 100,
                Comment = "Amazing product! Works perfectly!",
                Rating = 5
            };

            // Act - send POST request to create review
            var response = await _client.PostAsJsonAsync("/api/review/product", reviewRequest);

            // Assert - verify that the response is successful
            response.EnsureSuccessStatusCode();
            var review = await response.Content.ReadFromJsonAsync<ProductReview>();
            
            Assert.NotNull(review);
            Assert.Equal(1, review.ProductID);
            Assert.Equal(100, review.AccountID);
            Assert.Equal("Amazing product! Works perfectly!", review.Comment);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task PostProductReview_InvalidRating_ReturnsBadRequest()
        {
            // Arrange - rating of 10 is invalid and it must be 1-5
            var reviewRequest = new ProductReviewRequest
            {
                ProductID = 1,
                AccountID = 100,
                Comment = "Test",
                Rating = 10  // this will be invalid
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/review/product", reviewRequest);

            // Assert - should return 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllProductReviews_ReturnsEmptyList_WhenNoReviews()
        {
            // Act - get all reviews when none exist
            var response = await _client.GetAsync("/api/review/product");

            // Assert
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<ProductReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Empty(reviews);
        }

        [Fact]
        public async Task GetAllProductReviews_ReturnsReviews_AfterCreating()
        {
            // Arrange - create 2 reviews
            var review1 = new ProductReviewRequest { ProductID = 1, AccountID = 1, Comment = "Good", Rating = 4 };
            var review2 = new ProductReviewRequest { ProductID = 2, AccountID = 2, Comment = "Great", Rating = 5 };
            
            await _client.PostAsJsonAsync("/api/review/product", review1);
            await _client.PostAsJsonAsync("/api/review/product", review2);

            // Act - get all reviews
            var response = await _client.GetAsync("/api/review/product");

            // Assert
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<ProductReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Equal(2, reviews.Length);
        }

        [Fact]
        public async Task GetProductReviewsById_ReturnsOnlyMatchingReviews()
        {
            // Arrange - create reviews for different products
            var review1 = new ProductReviewRequest { ProductID = 1, AccountID = 1, Comment = "Product 1 review", Rating = 5 };
            var review2 = new ProductReviewRequest { ProductID = 1, AccountID = 2, Comment = "Another Product 1 review", Rating = 4 };
            var review3 = new ProductReviewRequest { ProductID = 2, AccountID = 1, Comment = "Product 2 review", Rating = 3 };
            
            await _client.PostAsJsonAsync("/api/review/product", review1);
            await _client.PostAsJsonAsync("/api/review/product", review2);
            await _client.PostAsJsonAsync("/api/review/product", review3);

            // Act - get reviews for Product 1 only
            var response = await _client.GetAsync("/api/review/product/1");

            // Assert - should only get 2 reviews for Product 1
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<ProductReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Equal(2, reviews.Length);
            Assert.All(reviews, r => Assert.Equal(1, r.ProductID));
        }

        [Fact]
        public async Task GetProductAverageRating_CalculatesCorrectly()
        {
            // Arrange - create 3 reviews with ratings: 3, 4, 5 which gives average of 4.0
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 1, AccountID = 1, Comment = "OK", Rating = 3 });
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 1, AccountID = 2, Comment = "Good", Rating = 4 });
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 1, AccountID = 3, Comment = "Great", Rating = 5 });

            // Act - get average rating
            var response = await _client.GetAsync("/api/review/product/1/average");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AverageRatingResponse>();
            
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductID);
            Assert.Equal(4.0, result.AverageRating);
        }

        [Fact]
        public async Task GetProductAverageRating_NoReviews_ReturnsZero()
        {
            // Act - get average for product with no reviews
            var response = await _client.GetAsync("/api/review/product/999/average");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AverageRatingResponse>();
            
            Assert.NotNull(result);
            Assert.Equal(999, result.ProductID);
            Assert.Equal(0.0, result.AverageRating);
        }

        //  SERVICE REVIEW TESTS 

        [Fact]
        public async Task PostServiceReview_ValidData_ReturnsCreatedReview()
        {
            // Arrange
            var reviewRequest = new ServiceReviewRequest
            {
                ServiceID = 1,
                AccountID = 100,
                Comment = "Excellent service! Very professional!",
                Rating = 5
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/review/service", reviewRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var review = await response.Content.ReadFromJsonAsync<ServiceReview>();
            
            Assert.NotNull(review);
            Assert.Equal(1, review.ServiceID);
            Assert.Equal(100, review.AccountID);
            Assert.Equal("Excellent service! Very professional!", review.Comment);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task PostServiceReview_InvalidRating_ReturnsBadRequest()
        {
            // Arrange - rating of 0 is invalid
            var reviewRequest = new ServiceReviewRequest
            {
                ServiceID = 1,
                AccountID = 100,
                Comment = "Test",
                Rating = 0  // this swill be invalid
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/review/service", reviewRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllServiceReviews_ReturnsAllReviews()
        {
            // Arrange - create 2 service reviews
            var review1 = new ServiceReviewRequest { ServiceID = 1, AccountID = 1, Comment = "Good service", Rating = 4 };
            var review2 = new ServiceReviewRequest { ServiceID = 2, AccountID = 2, Comment = "Great service", Rating = 5 };
            
            await _client.PostAsJsonAsync("/api/review/service", review1);
            await _client.PostAsJsonAsync("/api/review/service", review2);

            // Act
            var response = await _client.GetAsync("/api/review/service");

            // Assert
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<ServiceReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Equal(2, reviews.Length);
        }

        [Fact]
        public async Task GetServiceReviewsById_ReturnsOnlyMatchingReviews()
        {
            // Arrange - create reviews for different services
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 1, AccountID = 1, Comment = "Service 1", Rating = 5 });
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 1, AccountID = 2, Comment = "Service 1 again", Rating = 4 });
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 2, AccountID = 1, Comment = "Service 2", Rating = 3 });

            // Act - get only Service 1 reviews
            var response = await _client.GetAsync("/api/review/service/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var reviews = await response.Content.ReadFromJsonAsync<ServiceReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Equal(2, reviews.Length);
            Assert.All(reviews, r => Assert.Equal(1, r.ServiceID));
        }

        [Fact]
        public async Task GetServiceAverageRating_CalculatesCorrectly()
        {
            // Arrange - create reviews with ratings: 4, 5 with an average of 4.5
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 1, AccountID = 1, Comment = "Good", Rating = 4 });
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 1, AccountID = 2, Comment = "Great", Rating = 5 });

            // Act
            var response = await _client.GetAsync("/api/review/service/1/average");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AverageRatingResponse>();
            
            Assert.NotNull(result);
            Assert.Equal(1, result.ServiceID);
            Assert.Equal(4.5, result.AverageRating);
        }

        //  EMAIL REQUEST TESTS 

        [Fact]
        public async Task PostRequestReview_ValidData_ReturnsSuccess()
        {
            //  This test will actually send an email if email service is configured and 
            // In a real test environment WE would actually just mock the email service
            
            // Arrange
            var emailRequest = new ReviewRequestDTO
            {
                CustomerEmail = "test@example.com",
                CustomerName = "Test Customer",
                ProductID = 1,
                ServiceID = null,
                AccountID = 100,
                TransactionID = "TEST-12345"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/review/request-review", emailRequest);

            // Assert - check status code 
            Assert.True(response.StatusCode == HttpStatusCode.OK || 
                       response.StatusCode == HttpStatusCode.BadRequest);
        }

        // ==================== EDGE CASE TESTS ====================

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task PostProductReview_AllValidRatings_Succeeds(int rating)
        {
            // Test that all valid ratings (1-5) work correctly
            var reviewRequest = new ProductReviewRequest
            {
                ProductID = 1,
                AccountID = 100,
                Comment = $"Review with rating {rating}",
                Rating = rating
            };

            var response = await _client.PostAsJsonAsync("/api/review/product", reviewRequest);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task MultipleReviews_FromSameUser_AllSaved()
        {
            // Test that same user can leave multiple reviews for different products
            var user = 100;
            
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 1, AccountID = user, Comment = "Product 1", Rating = 5 });
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 2, AccountID = user, Comment = "Product 2", Rating = 4 });
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 3, AccountID = user, Comment = "Product 3", Rating = 3 });

            // Get all reviews
            var response = await _client.GetAsync("/api/review/product");
            var reviews = await response.Content.ReadFromJsonAsync<ProductReview[]>();
            
            Assert.NotNull(reviews);
            Assert.Equal(3, reviews.Length);
            Assert.All(reviews, r => Assert.Equal(user, r.AccountID));
        }

        [Fact]
        public async Task ProductAndServiceReviews_StaySeparate()
        {
            // Test that product and service reviews do not mix
            await _client.PostAsJsonAsync("/api/review/product", 
                new ProductReviewRequest { ProductID = 1, AccountID = 1, Comment = "Product", Rating = 5 });
            await _client.PostAsJsonAsync("/api/review/service", 
                new ServiceReviewRequest { ServiceID = 1, AccountID = 1, Comment = "Service", Rating = 4 });

            // Check both endpoints
            var productResponse = await _client.GetAsync("/api/review/product");
            var serviceResponse = await _client.GetAsync("/api/review/service");
            
            var productReviews = await productResponse.Content.ReadFromJsonAsync<ProductReview[]>();
            var serviceReviews = await serviceResponse.Content.ReadFromJsonAsync<ServiceReview[]>();
            
            Assert.Single(productReviews);
            Assert.Single(serviceReviews);
            Assert.Equal(5, productReviews[0].Rating);
            Assert.Equal(4, serviceReviews[0].Rating);
        }

        // Helper class for deserializing average rating responses
        private class AverageRatingResponse
        {
            public int ProductID { get; set; }
            public int ServiceID { get; set; }
            public double AverageRating { get; set; }
        }
    }
}
