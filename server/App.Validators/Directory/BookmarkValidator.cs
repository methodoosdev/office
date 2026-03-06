using App.Data;
using App.Models.Directory;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class BookmarkValidator : BaseNopValidator<BookmarkModel>
    {
        public BookmarkValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.UrlPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.BookmarkModel.Validation.UrlPath"));
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.BookmarkModel.Validation.Description"));
        }
    }
}