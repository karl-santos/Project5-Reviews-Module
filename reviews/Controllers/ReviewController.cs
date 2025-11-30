using Microsoft.AspNetCore.Mvc;
using reviews.Models;
using reviews.Services;

namespace reviews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly ReviewService _reviewService;
        private readonly EmailService _emailService;

        public ReviewController(ILogger<ReviewController> logger, ReviewService reviewService, EmailService emailService)
        {
            _logger = logger;
            _reviewService = reviewService;
            _emailService = emailService;
        }

        // POST: api/review/request-review - AUTOMATED EMAIL TRIGGER FOR POS
        // POS calls this after customer completes purchase - email sends automatically!
        [HttpPost("request-review")]
        public ActionResult RequestReview([FromBody] ReviewRequestDTO request)
        {
            try
            {
                // Send automated email to customer with review link
                _emailService.SendReviewRequestEmail(
                    request.CustomerEmail,
                    request.CustomerName,
                    request.ProductID,
                    request.ServiceID,
                    request.AccountID,
                    request.TransactionID
                );

                return Ok(new { 
                    message = "Review request email sent successfully!", 
                    sentTo = request.CustomerEmail 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send review request email");
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/review/product - Submit a product review
        [HttpPost("product")]
        public ActionResult<ProductReview> AddProductReview([FromBody] ProductReviewRequest request)
        {
            try
            {
                var review = _reviewService.AddProductReview(
                    request.ProductID,
                    request.AccountID,
                    request.Comment,
                    request.Rating);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/review/service - Submit a service review
        [HttpPost("service")]
        public ActionResult<ServiceReview> AddServiceReview([FromBody] ServiceReviewRequest request)
        {
            try
            {
                var review = _reviewService.AddServiceReview(
                    request.ServiceID,
                    request.AccountID,
                    request.Comment,
                    request.Rating);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/review/product - Get all product reviews
        [HttpGet("product")]
        public ActionResult<List<ProductReview>> GetAllProductReviews()
        {
            return Ok(_reviewService.ProductReviews);
        }

        // GET: api/review/service - Get all service reviews
        [HttpGet("service")]
        public ActionResult<List<ServiceReview>> GetAllServiceReviews()
        {
            return Ok(_reviewService.ServiceReviews);
        }

        // GET: api/review/product/{productId} - Get reviews for specific product
        [HttpGet("product/{productId}")]
        public ActionResult<List<ProductReview>> GetProductReviews(int productId)
        {
            var reviews = _reviewService.ProductReviews
                .Where(r => r.ProductID == productId)
                .ToList();
            return Ok(reviews);
        }

        // GET: api/review/service/{serviceId} - Get reviews for specific service
        [HttpGet("service/{serviceId}")]
        public ActionResult<List<ServiceReview>> GetServiceReviews(int serviceId)
        {
            var reviews = _reviewService.ServiceReviews
                .Where(r => r.ServiceID == serviceId)
                .ToList();
            return Ok(reviews);
        }

        // GET: api/review/product/{productId}/average - Get average product rating
        [HttpGet("product/{productId}/average")]
        public ActionResult<double> GetAverageProductRating(int productId)
        {
            var average = _reviewService.GetAverageProductRating(productId);
            return Ok(new { ProductID = productId, AverageRating = average });
        }

        // GET: api/review/service/{serviceId}/average - Get average service rating
        [HttpGet("service/{serviceId}/average")]
        public ActionResult<double> GetAverageServiceRating(int serviceId)
        {
            var average = _reviewService.GetAverageServiceRating(serviceId);
            return Ok(new { ServiceID = serviceId, AverageRating = average });
        }
    }

    // Request DTOs - Simple data classes for API requests
    
    // DTO for POS to trigger automated review email
    public class ReviewRequestDTO
    {
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public int? ProductID { get; set; }      // Set this for product reviews
        public int? ServiceID { get; set; }      // Set this for service reviews
        public int AccountID { get; set; }
        public string TransactionID { get; set; }
    }

    public class ProductReviewRequest
    {
        public int ProductID { get; set; }
        public int AccountID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }

    public class ServiceReviewRequest
    {
        public int ServiceID { get; set; }
        public int AccountID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
