namespace TheGameVoice.Web.Areas.Admin.ViewModels.Media;

public class MediaPickerItemViewModel
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = default!;

    public string FilePath { get; set; } = default!;
}