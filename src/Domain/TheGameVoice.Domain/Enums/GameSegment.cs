namespace TheGameVoice.Domain.Enums;

/// <summary>
/// Audience segment a piece of content belongs to.
///
/// This is NOT a publishing channel (web vs app) and it is NOT
/// <see cref="Entities.Game.Platforms"/> (the free text "PS5, Xbox, PC" field).
/// It answers a single editorial question: is this content about
/// PC / console gaming, mobile gaming, or both?
///
/// The public site renders one segment at a time and the reader flips
/// between them with the toggle in the header.
/// </summary>
[Flags]
public enum GameSegment
{
    None = 0,

    PcConsole = 1,

    Mobile = 2,

    All = PcConsole | Mobile
}
