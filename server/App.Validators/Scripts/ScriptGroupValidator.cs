using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptGroupValidator : BaseNopValidator<ScriptGroupModel>
    {
        public ScriptGroupValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.GroupName).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptGroupModel.Validation.GroupName"));            
        }
    }
}