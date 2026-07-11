using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Application.Settings;

namespace TheGameVoice.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string html)
    {
        using var client = new SmtpClient(
            _settings.Host,
            _settings.Port);

        client.EnableSsl = true;

        client.Credentials =
            new NetworkCredential(
                _settings.Email,
                _settings.Password);

        var message = new MailMessage
        {
            From = new MailAddress(
                _settings.Email,
                _settings.DisplayName),

            Subject = subject,

            Body = html,

            IsBodyHtml = true
        };

        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}