using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Media;

public class UploadMediaViewModel
{
    [Required]
    public IFormFile File { get; set; }
        = default!;
}