using TheGameVoice.Application.Modules.Tags;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Tags;

public class TagIndexViewModel
{
    public string Search { get; set; } = string.Empty;
    public string Usage { get; set; } = "all";
    public string Sort { get; set; } = "name";
    public TagAdminPage Result { get; set; } = new();
}

public class EditTagViewModel : CreateTagViewModel
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
}
