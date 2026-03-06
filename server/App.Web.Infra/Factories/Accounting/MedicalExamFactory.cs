using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IMedicalExamFactory
    {
        Task<MedicalExamTableModel> PrepareMedicalExamTableModelAsync(MedicalExamTableModel tableModel);
    }
    public class MedicalExamFactory : IMedicalExamFactory
    {
        private readonly ILocalizationService _localizationService;

        public MedicalExamFactory(
            ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public virtual async Task<MedicalExamTableModel> PrepareMedicalExamTableModelAsync(MedicalExamTableModel tableModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MedicalExamModel>(1, nameof(MedicalExamModel.Exam)),
                ColumnConfig.Create<MedicalExamModel>(2, nameof(MedicalExamModel.Category)),
                ColumnConfig.Create<MedicalExamModel>(5, nameof(MedicalExamModel.Symmetochi), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<MedicalExamModel>(5, nameof(MedicalExamModel.Foreas), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<MedicalExamModel>(5, nameof(MedicalExamModel.Price), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<MedicalExamModel>(10, nameof(MedicalExamModel.Count), style: rightAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MedicalExamModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}