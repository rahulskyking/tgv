using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Application.Helpers;

public static class ReviewHelper
{
    public static ReviewVerdict GetVerdict(decimal score)
    {
        if (score >= 9.5m)
            return ReviewVerdict.Masterpiece;

        if (score >= 8.5m)
            return ReviewVerdict.Amazing;

        if (score >= 7.5m)
            return ReviewVerdict.Great;

        if (score >= 6.5m)
            return ReviewVerdict.Good;

        if (score >= 5.5m)
            return ReviewVerdict.Fair;

        if (score >= 4.5m)
            return ReviewVerdict.Average;

        if (score >= 3.5m)
            return ReviewVerdict.Bad;

        if (score >= 2.5m)
            return ReviewVerdict.Poor;

        if (score >= 1.5m)
            return ReviewVerdict.Awful;

        return ReviewVerdict.Worst;
    }
}