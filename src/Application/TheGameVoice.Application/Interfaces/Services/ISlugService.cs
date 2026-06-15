namespace TheGameVoice.Application.Interfaces.Services;

public interface ISlugService
{
    Task<string> GenerateSlugAsync(string title);
    Task<string> GenerateAuthorSlugAsync(
    string fullName);
}