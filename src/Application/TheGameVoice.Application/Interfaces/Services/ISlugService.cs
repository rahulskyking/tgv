namespace TheGameVoice.Application.Interfaces.Services;

public interface ISlugService
{
    Task<string> GenerateSlugAsync(string title);
}