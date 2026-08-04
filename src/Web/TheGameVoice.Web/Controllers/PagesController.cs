using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

    [Route("socials")]
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

}