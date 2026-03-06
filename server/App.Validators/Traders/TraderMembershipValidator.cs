using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderMembershipValidator : BaseNopValidator<TraderMembershipModel>
    {
        public TraderMembershipValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ParentId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderMembershipModel.Validation.ParentId"));
            RuleFor(x => x.Vat).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderMembershipModel.Validation.Vat"));
            RuleFor(x => x.SurnameFatherName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderMembershipModel.Validation.SurnameFatherName"));
            RuleFor(x => x.ParticipationName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderMembershipModel.Validation.ParticipationName"));
            RuleFor(x => x.TraderBoardMemberTypeId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderMembershipModel.Validation.TraderBoardMemberTypeId"));

        }
    }
}