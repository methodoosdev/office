using App.Data;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Localization;
using FluentValidation;
using LinqToDB;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App.Validators
{
    public partial class DepartmentValidator : BaseNopValidator<DepartmentModel>
    {
        public DepartmentValidator(ILocalizationService localizationService, 
            IDepartmentService departmentService,
            INopDataProvider dataProvider)
        {
            RuleFor(x => x.SystemName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.DepartmentModel.Validation.SystemName"));
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.DepartmentModel.Validation.Description"));

            RuleFor(x => x.SystemName)
            .Must((x, value) => 
            {
                if (x.Id > 0)
                    return true;

                return !departmentService.Table.Any(c => c.SystemName == value);
            }).WithMessageAwait(localizationService.GetResourceAsync("App.Models.DepartmentModel.Validation.SystemNameUnique"));
            
            //RuleFor(x => x.Description).NotEmpty().WithMessageAwait(async (x, context) =>
            //{
            //    var property = string.Format("App.Models.{0}.Fields.{1}", x.GetType().Name, nameof(x.Description));
            //    var notEmpty = await localizationService.GetResourceAsync("App.Common.Validation.NotEmpty");

            //    return string.Format(notEmpty, await localizationService.GetResourceAsync(property));
            //});
        }
    }
}