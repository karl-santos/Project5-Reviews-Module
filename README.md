# Reviews Module 

An ASP.NET Core application for managing product and service reviews with automated email notifications and MySQL database integration.

##  Project Structure

```
Project5-Reviews-Module/
├── reviews/                       # Main API project
│   ├── Controllers/
│   │   └── ReviewController.cs   # API endpoints
│   ├── Models/
│   │   └── ReviewBase.cs         # Product/Service review models
│   ├── Services/
│   │   ├── ReviewDbContext.cs    # Database connection
│   │   ├── ReviewService.cs      # Business logic
│   │   └── EmailService.cs       # Email automation
│   ├── wwwroot/                  # Frontend files
│   │   ├── index.html           # Review form UI
│   │   ├── app.js               # Frontend logic
│   │   └── style.css            # Styling
│   ├── appsettings.json         # Configuration
│   └── Program.cs               # App startup
├── reviews.Tests/               # Unit tests
│   ├── UnitTest1.cs            # Test suite
│   └── reviews.Tests.csproj
└── README.md                   # This file
```


## Database Schema

### ProductReview Table
```sql
CREATE TABLE ProductReview (
    ReviewID INT AUTO_INCREMENT PRIMARY KEY,
    ProductID INT NOT NULL,
    AccountID INT NOT NULL,
    Rating INT,
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

### ServiceReview Table
```sql
CREATE TABLE ServiceReview (
    ReviewID INT AUTO_INCREMENT PRIMARY KEY,
    ServiceID INT NOT NULL,
    AccountID INT NOT NULL,
    Rating INT,
    Comment TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

