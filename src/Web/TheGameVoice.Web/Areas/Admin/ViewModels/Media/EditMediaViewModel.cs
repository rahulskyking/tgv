namespace TheGameVoice.Web.Areas.Admin.ViewModels.Media
{
    public class EditMediaViewModel
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = default!;

        public string FilePath { get; set; } = default!;

        public string? AltText { get; set; }

        public string? Caption { get; set; }

        public string? Credit { get; set; }
    }
}
