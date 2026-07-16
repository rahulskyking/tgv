namespace TheGameVoice.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        string? replyTo = null,
        CancellationToken cancellationToken = default);
}