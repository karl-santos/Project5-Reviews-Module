using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace reviews.Services
{
    // email service for sending review requests
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly string _senderName;
        private readonly string _baseUrl;

        // Constructor 
        public EmailService(IConfiguration config)
        {
            _smtpServer = config["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(config["Email:SmtpPort"] ?? "587");
            _senderEmail = config["Email:SenderEmail"] ?? "";
            _senderPassword = config["Email:SenderPassword"] ?? "";
            _senderName = config["Email:SenderName"] ?? "Reviews Module";
            _baseUrl = config["AppSettings:BaseUrl"] ?? "http://localhost:5033";
        }

        // Send a review request email to a customer
        public void SendReviewRequestEmail(string toEmail, string productId, string orderId)
        {
            try
            {
                
                if (string.IsNullOrEmpty(_senderEmail) || string.IsNullOrEmpty(_senderPassword))
                {
                    Console.WriteLine($"[EMAIL] Would send to: {toEmail} for Product: {productId}, Order: {orderId}");
                    return;
                }

                // Create the email 
                var mail = new MailMessage();
                mail.From = new MailAddress(_senderEmail, _senderName);
                mail.To.Add(toEmail);
                mail.Subject = "We'd love your feedback!";

                // Email body with review link 
                string reviewUrl = $"{_baseUrl}/index.html?productId={productId}&orderId={orderId}";
                mail.Body = $@"
Hi there!

Thank you for your recent purchase (Order #{orderId})!

We really appreciate if you could take a moment to leave a review for the product you bought.

Click here to leave your review: {reviewUrl}

Your feedback helps us and other customers!
Thanks,
Reviews Team
                ";

                // Set up SMTP client
                var smtpClient = new SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    EnableSsl = true
                };

                // Send the email
                smtpClient.Send(mail);
                Console.WriteLine($"[EMAIL] Sent review request to {toEmail}");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"[EMAIL ERROR] Failed to send: {ex.Message}");
            }
        }
    }
}
