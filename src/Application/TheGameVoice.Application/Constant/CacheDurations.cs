namespace TheGameVoice.Application.Constants;

public static class CacheDurations
{
    public static readonly TimeSpan
        Short =
            TimeSpan.FromMinutes(5);

    public static readonly TimeSpan
        Medium =
            TimeSpan.FromMinutes(15);

    public static readonly TimeSpan
        Long =
            TimeSpan.FromHours(1);
}