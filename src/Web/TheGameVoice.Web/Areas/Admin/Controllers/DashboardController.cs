using Microsoft.AspNetCore.Mvc;

namespace TheGameVoice.Web.Areas.Admin.Controllers;

public class DashboardController
    : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}