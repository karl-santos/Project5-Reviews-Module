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

        public ReviewController(ILogger<ReviewController> logger, ReviewService reviewService)
        {
            _logger = logger;
            _reviewService = reviewService;
        }

        // POST: api/review/product
        [HttpPost("product")]
        public ActionResult<ProductReview> AddProductReview([FromBody] ProductReviewRequest request)
        {
            try
            {
                var review = _reviewService.AddProductReview(
                    request.ProductID,
                    request.UserID,
                    request.Comment,
                    request.Rating);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/review/rental
        [HttpPost("rental")]
        public ActionResult<RentalReview> AddRentalReview([FromBody] RentalReviewRequest request)
        {
            try
            {
                var review = _reviewService.AddRentalReview(
                    request.RentalID,
                    request.UserID,
                    request.Comment,
                    request.Rating);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/review/team
        [HttpPost("team")]
        public ActionResult<TeamReview> AddTeamReview([FromBody] TeamReviewRequest request)
        {
            try
            {
                var review = _reviewService.AddTeamReview(
                    request.TeamID,
                    request.UserID,
                    request.Comment,
                    request.Rating);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/review/product/{productId}/average
        [HttpGet("product/{productId}/average")]
        public ActionResult<double> GetAverageProductRating(string productId)
        {
            var average = _reviewService.GetAverageProductRating(productId);
            return Ok(new { ProductID = productId, AverageRating = average });
        }

        // GET: api/review/product (get all product reviews)
        [HttpGet("product")]
        public ActionResult<List<ProductReview>> GetAllProductReviews()
        {
            return Ok(_reviewService.ProductReviews);
        }

        // GET: api/review/rental (get all rental reviews)
        [HttpGet("rental")]
        public ActionResult<List<RentalReview>> GetAllRentalReviews()
        {
            return Ok(_reviewService.RentalReviews);
        }

        // GET: api/review/team (get all team reviews)
        [HttpGet("team")]
        public ActionResult<List<TeamReview>> GetAllTeamReviews()
        {
            return Ok(_reviewService.TeamReviews);
        }

        // GET: api/review/product/{productId} (get reviews for specific product)
        [HttpGet("product/{productId}")]
        public ActionResult<List<ProductReview>> GetProductReviews(string productId)
        {
            var reviews = _reviewService.ProductReviews
                .Where(r => r.ProductID == productId)
                .ToList();
            return Ok(reviews);
        }
    }

    // Request DTOs
    public class ProductReviewRequest
    {
        public string ProductID { get; set; }
        public string UserID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }

    public class RentalReviewRequest
    {
        public string RentalID { get; set; }
        public string UserID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }

    public class TeamReviewRequest
    {
        public string TeamID { get; set; }
        public string UserID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}