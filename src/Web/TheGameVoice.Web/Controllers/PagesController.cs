using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Application.Settings;
using TheGameVoice.Web.ViewModels.Pages;
namespace TheGameVoice.Web.Controllers;
public class PagesController : Controller
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    public PagesController(
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings)
    {
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }
    [Route("about")]
    public IActionResult About()
    {
        return View();
    }
    [HttpGet]
    [Route("contact")]
    public IActionResult Contact()
    {
        return View(new ContactViewModel
        {
            ContactEmail = _emailSettings.Email
        });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("contact")]
    public async Task<IActionResult> Contact(ContactViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            return RedirectToAction(nameof(Contact));
        }
        if (!ModelState.IsValid)
        {
            model.ContactEmail = _emailSettings.Email;
            return View(model);
        }
        var body = $@"
<h2>New Contact Message</h2>
<p><strong>Name:</strong> {model.Name}</p>
<p><strong>Email:</strong> {model.Email}</p>
<p><strong>Subject:</strong> {model.Subject}</p>
<hr>
<p>{model.Message}</p>";
        try
        {
            await _emailService.SendAsync(
                _emailSettings.Email,
                $"TheGameVoice Contact - {model.Subject}",
                body,
                replyTo: model.Email);
            TempData["Success"] = "Thank you! Your message has been sent successfully.";
        }
        catch (Exception ex)
        {
            // TODO: inject ILogger<PagesController> and log ex here
            TempData["Error"] = "Sorry, something went wrong sending your message. Please try again shortly.";
        }
        return RedirectToAction(nameof(Contact));
    }
    [Route("review-policy")]
    public IActionResult ReviewPolicy()
    {
        return View();
    }

    #region TEMP_DEBUG_EMAIL — remove before final deploy

    [HttpGet]
    [Route("debug/email-config")]
    public IActionResult DebugEmailConfig()
    {
        return Json(new
        {
            Host = _emailSettings.Host,
            Port = _emailSettings.Port,
            Email = _emailSettings.Email,
            PasswordLength = _emailSettings.Password?.Length ?? 0, // never expose the actual password
            DisplayName = _emailSettings.DisplayName
        });
    }

    [HttpGet]
    [Route("debug/smtp-send")]
    public async Task<IActionResult> DebugSmtpSend()
    {
        var log = new List<string>();

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
            message.To.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email)); // sends to yourself
            message.Subject = $"Debug test - {DateTime.UtcNow:O}";
            message.Body = new TextPart("plain") { Text = "This is a debug test email from /debug/smtp-send." };

            using var client = new SmtpClient { Timeout = 15000 };
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            log.Add($"[1] Connecting to {_emailSettings.Host}:{_emailSettings.Port}...");
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls, cts.Token);
            log.Add("[2] Connected. Authenticating...");

            await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password, cts.Token);
            log.Add("[3] Authenticated. Sending...");

            await client.SendAsync(message, cts.Token);
            log.Add("[4] Sent successfully.");

            await client.DisconnectAsync(true, CancellationToken.None);
            log.Add("[5] Disconnected cleanly.");

            return Json(new { Success = true, Log = log });
        }
        catch (Exception ex)
        {
            log.Add($"[ERROR] {ex.GetType().Name}: {ex.Message}");
            return Json(new
            {
                Success = false,
                ExceptionType = ex.GetType().Name,
                ex.Message,
                Log = log
            });
        }
    }

    #endregion

}