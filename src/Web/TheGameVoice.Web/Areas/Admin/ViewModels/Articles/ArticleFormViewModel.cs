using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TheGameVoice.Domain.Enums;
using TheGameVoice.Web.Areas.Admin.ViewModels.Media;

namespace TheGameVoice.Web.Areas.Admin.ViewModels.Articles;

public class ArticleFormViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Article title is required.")]
    [StringLength(300,
        MinimumLength = 10,
        ErrorMessage = "Title must be between 10 and 300 characters.")]
    public string Title { get; set; } = default!;

    [Required(ErrorMessage = "Summary is required.")]
    [StringLength(500,
        MinimumLength = 30,
        ErrorMessage = "Summary must be between 10 and 500 characters.")]
    public string Summary { get; set; } = default!;

    [Required(ErrorMessage = "Content cannot be empty.")]
    public string Content { get; set; } = default!;

    // Hidden from UI but kept for future SEO
    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public Guid? FeaturedImageId { get; set; }

    [Required(ErrorMessage = "Please select a category.")]
    public Guid CategoryId { get; set; }

    public List<Guid> SelectedTagIds { get; set; } = new();

    public List<Guid> SelectedGameIds { get; set; } = new();

    public string? FeaturedImagePath { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();

    public List<SelectListItem> Tags { get; set; } = new();

    public List<SelectListItem> Games { get; set; } = new();

    public List<MediaPickerItemViewModel> MediaItems { get; set; } = new();

    public string StatusDisplay { get; set; } = "Draft";

    [Required(ErrorMessage = "Please select an author.")]
    public Guid AuthorId { get; set; }

    public List<SelectListItem> Authors { get; set; } = new();

    public List<Guid> SelectedGalleryImageIds { get; set; } = new();

    public List<MediaPickerItemViewModel> GalleryImages { get; set; } = new();

    public List<ArticleVideoInputViewModel> Videos { get; set; } = new();

    #region Review

    public bool IsReview { get; set; }

    [Range(1, 10,
        ErrorMessage = "Review score must be between 1 and 10.")]
    public decimal? ReviewScore { get; set; }

    public ReviewVerdict? ReviewVerdict { get; set; }

    [StringLength(500,
        ErrorMessage = "Review summary cannot exceed 500 characters.")]
    public string? ReviewSummary { get; set; }

    public List<string> GoodReviewPoints { get; set; } = new();

    public List<string> BadReviewPoints { get; set; } = new();

    #endregion

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        // Title validation
        if (!string.IsNullOrWhiteSpace(Title) &&
            Title.Trim().Length < 10)
        {
            yield return new ValidationResult(
                "Title should contain at least 10 characters.",
                new[] { nameof(Title) });
        }

        // Summary validation
        if (!string.IsNullOrWhiteSpace(Summary) &&
            Summary.Trim().Length < 10)
        {
            yield return new ValidationResult(
                "Summary should contain at least 10 characters.",
                new[] { nameof(Summary) });
        }

        // Content validation
        if (string.IsNullOrWhiteSpace(Content))
        {
            yield return new ValidationResult(
                "Article content is required.",
                new[] { nameof(Content) });
        }

        // Category
        if (CategoryId == Guid.Empty)
        {
            yield return new ValidationResult(
                "Please select a category.",
                new[] { nameof(CategoryId) });
        }

        // Author
        if (AuthorId == Guid.Empty)
        {
            yield return new ValidationResult(
                "Please select an author.",
                new[] { nameof(AuthorId) });
        }

        // Featured Image
        if (FeaturedImageId == null)
        {
            yield return new ValidationResult(
                "Please select a featured image.",
                new[] { nameof(FeaturedImageId) });
        }

        // Review validation
        if (IsReview)
        {
            if (!ReviewScore.HasValue)
            {
                yield return new ValidationResult(
                    "Review score is required.",
                    new[] { nameof(ReviewScore) });
            }

            if (string.IsNullOrWhiteSpace(ReviewSummary))
            {
                yield return new ValidationResult(
                    "Review summary is required.",
                    new[] { nameof(ReviewSummary) });
            }

            if (!GoodReviewPoints.Any(x =>
                !string.IsNullOrWhiteSpace(x)))
            {
                yield return new ValidationResult(
                    "Please add at least one positive point.",
                    new[] { nameof(GoodReviewPoints) });
            }

            if (!BadReviewPoints.Any(x =>
                !string.IsNullOrWhiteSpace(x)))
            {
                yield return new ValidationResult(
                    "Please add at least one negative point.",
                    new[] { nameof(BadReviewPoints) });
            }
        }
    }
}