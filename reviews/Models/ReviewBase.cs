using Microsoft.AspNetCore.Mvc;

namespace reviews.Models
{

    // Base review properties
    public abstract class ReviewBase
    {
        public int ReviewID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string UserID { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // Product Review
    public class ProductReview : ReviewBase
    {
        public string ProductID { get; set; }
    }

    // Rental Review
    public class RentalReview : ReviewBase
    {
        public string RentalID { get; set; }
    }

    // Team Review
    public class TeamReview : ReviewBase
    {
        public string TeamID { get; set; }
    }


}
