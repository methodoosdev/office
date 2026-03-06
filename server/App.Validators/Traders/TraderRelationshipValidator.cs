using App.Core;
using App.Data;
using App.Models.Traders;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class TraderRelationshipValidator : BaseNopValidator<TraderRelationshipModel>
    {
        public TraderRelationshipValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ParentId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.Validation.ParentId"));
            RuleFor(x => x.Vat).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.Validation.Vat"));
            RuleFor(x => x.SurnameFatherName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.Validation.SurnameFatherName"));
            RuleFor(x => x.RelationshipName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.Validation.RelationshipName"));
            RuleFor(x => x.TraderBoardMemberTypeId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.Validation.TraderBoardMemberTypeId"));

        }
    }
}