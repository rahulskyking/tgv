namespace TheGameVoice.Application.Modules;

public class MediaMigrationResult
{
    public int Total { get; set; }

    public int Migrated { get; set; }

    public int Skipped { get; set; }

    public int Failed { get; set; }

    public List<string> Errors { get; set; } = new();
}