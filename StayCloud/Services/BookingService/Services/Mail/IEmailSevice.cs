using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string attachmentName, byte[] attachmentData);
}