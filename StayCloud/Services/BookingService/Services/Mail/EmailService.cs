using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
        {
            Port = int.Parse(emailSettings["Port"]!),
            Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(emailSettings["SenderEmail"]!, emailSettings["SenderName"]),
            Subject = subject,
            Body = body
        };

        mail.To.Add(toEmail);
        await smtpClient.SendMailAsync(mail);
    }

    public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string attachmentName, byte[] attachmentData)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
        {
            Port = int.Parse(emailSettings["Port"]!),
            Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(emailSettings["SenderEmail"]!, emailSettings["SenderName"]),
            Subject = subject,
            Body = body
        };

        mail.To.Add(toEmail);

        // Voeg bijlage toe
        var attachment = new Attachment(new MemoryStream(attachmentData), attachmentName);
        mail.Attachments.Add(attachment);

        await smtpClient.SendMailAsync(mail);
    }
}

