// Controllers/ContactController.cs
using FormSubmission_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public ContactController(IConfiguration configuration)
    {

        _configuration = configuration;

    }

    [HttpPost("send")]
    public IActionResult SendContactForm([FromForm] ContactFormModel form)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Configure the email settings
                var adminEmail = _configuration.GetValue<string>("AdminEmail");  // Replace with the actual admin email
                var subject = $"{form.Name} Shows interest on your Portfolio";
                var body = "Below are the Submitted details\n"+
                           $"Purpose: {form.Purpose}\n" +
                           $"Name: {form.Name}\n" +
                           $"Email: {form.Email}\n" +
                           $"Company: {form.Company}\n" +
                           $"Phone: {form.Phone}\n" +
                           $"Message: {form.Message}";


                // Set up the SMTP client
                var username = _configuration.GetValue<string>("SmtpServerName");
                using (var smtpClient = new SmtpClient(_configuration.GetValue<string>("Host")))
                {
                    smtpClient.Port = 587; // Change port if needed
                    smtpClient.Credentials = new NetworkCredential(username,_configuration.GetValue<string>("AppServiceKey")); // Set credentials
                    smtpClient.EnableSsl = true;



                    // Create the mail message
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(username!),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    if (form.Document != null)
                    {
                        MemoryStream memoryStream = new();
                        form.Document.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        var attachment = new Attachment(memoryStream, form.Document.Name, form.Document.ContentType);
                        mailMessage.Attachments.Add(attachment);
                    }
                    mailMessage.To.Add(adminEmail!);
                    // Send the email
                    smtpClient.Send(mailMessage);
                }

                return Ok(new { message = "Your message has been sent successfully!" });
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending email", error = ex.Message });
            }
        }

        return BadRequest(new { message = "Invalid form data" });
    }
}
