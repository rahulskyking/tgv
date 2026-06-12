using Microsoft.AspNetCore.Mvc;
using TheGameVoice.Application.Interfaces.Persistence;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Web.Controllers;

[ApiController]
[Route("api/article-views")]
public class ArticleViewsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ArticleViewsController(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost("{articleId:guid}")]
    public async Task<IActionResult> Track(
     Guid articleId)
    {
        await _unitOfWork.ArticleViews
            .AddAsync(
                new ArticleView
                {
                    ArticleId = articleId,
                    ViewedAt = DateTime.UtcNow
                });

        var article =
            await _unitOfWork.Articles
                .GetByIdAsync(articleId);

        if (article != null)
        {
            article.ViewCount++;

            _unitOfWork.Articles
                .Update(article);
        }

        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }
}