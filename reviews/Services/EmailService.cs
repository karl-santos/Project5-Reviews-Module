using MailKit.Net.Smtp;
using MimeKit;

namespace reviews.Services
{
    // Email service - sends automated review request emails to customers
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;
        private readonly string _baseUrl;

        public EmailService(IConfiguration config)
        {
            // Load email settings from appsettings.json
            _smtpServer = config["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(config["Email:SmtpPort"] ?? "587");
            _senderEmail = config["Email:SenderEmail"] ?? "";
            _senderPassword = config["Email:SenderPassword"] ?? "";
            _senderName = config["Email:SenderName"] ?? "Reviews Team";
            _baseUrl = config["AppSettings:BaseUrl"] ?? "http://localhost:5033";
        }

        // Send review request email to customer
        public void SendReviewRequestEmail(string toEmail, string customerName, int? productId, int? serviceId, int accountId, string transactionId)
        {
            try
            {
                // Create email message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_senderName, _senderEmail));
                message.To.Add(new MailboxAddress(customerName, toEmail));
                message.Subject = "We'd love your feedback! ⭐";

                // Build review link (pre-fills form with customer info)
                string reviewUrl;
                if (productId.HasValue)
                {
                    reviewUrl = $"{_baseUrl}/index.html?productId={productId}&accountId={accountId}&type=product";
                }
                else if (serviceId.HasValue)
                {
                    reviewUrl = $"{_baseUrl}/index.html?serviceId={serviceId}&accountId={accountId}&type=service";
                }
                else
                {
                    throw new ArgumentException("Either productId or serviceId must be provided");
                }

                // Email body
                message.Body = new TextPart("html")
                {
                    Text = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .button {{ 
            display: inline-block; 
            padding: 12px 30px; 
            background-color: #4CAF50; 
            color: white; 
            text-decoration: none; 
            border-radius: 5px; 
            margin: 20px 0;
        }}
        .footer {{ font-size: 12px; color: #666; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Hi {customerName}! 👋</h2>
        <p>Thank you for your recent purchase (Transaction #{transactionId})!</p>
        <p>We'd really appreciate if you could take a moment to share your experience.</p>
        <p>
            <a href='{reviewUrl}' class='button'>Leave Your Review ⭐</a>
        </p>
        <p>Your feedback is DEEPLYYYYYYYYYYYYYY RTAHHHHH Appreciated</p>
        <div class='footer'>
            <p>Thanks for being chill fam 😊<br>- The Reviews Team</p>
        </div>
    </div>
</body>
</html>
"
                };

                // Send email using Gmail SMTP
                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(_senderEmail, _senderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }

                Console.WriteLine($"✅ Review email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
