namespace TheGameVoice.Infrastructure.Configuration;

public class SupabaseStorageOptions
{
    public string ProjectUrl
    {
        get;
        set;
    } = default!;

    public string BucketName
    {
        get;
        set;
    } = "media";

    public string ServiceRoleKey
    {
        get;
        set;
    } = default!;
}