using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Models.Payroll;
using App.Services;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IApdSubmissionModelFactory
    {
        Task<ApdSubmissionModel> ExtractApdSubmissionModel(bool found, string pdfText, string traderName);
        Task<ApdSubmissionSearchModel> PrepareApdSubmissionSearchModelAsync(ApdSubmissionSearchModel searchModel);
        Task<ApdSubmissionTableModel> PrepareApdSubmissionTableModelAsync(ApdSubmissionTableModel tableModel);
    }
    public partial class ApdSubmissionModelFactory : IApdSubmissionModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;

        public ApdSubmissionModelFactory(
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService)
        {
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
        }

        public async Task<ApdSubmissionModel> ExtractApdSubmissionModel(bool found, string pdfText, string traderName)
        {
            var model = new ApdSubmissionModel();
            if (found)
            {
                var lines = pdfText.Split('\n').ToList();
                lines.RemoveAt(0);

                var date = lines[0].Split(':');
                var period = lines[17].Split('/');
                var month = period[0];
                var year = period[1];

                model.SubmissionNumber = lines[27];
                model.Type = lines[3];
                model.Ame = lines[9];
                model.Surname = lines[7];
                model.Vat = lines[10];
                model.Period = lines[17];
                model.Month = month;
                model.Year = year;
                model.Amoe = "";
                model.TotalInsuranceDays = int.Parse(lines[19]);
                model.TotalEarnings = decimal.Parse(lines[21], new CultureInfo("el-GR"));
                model.TotalContributions = decimal.Parse(lines[23], new CultureInfo("el-GR"));
                model.SubmissionDate = date[1].Trim();
                model.Tpte = lines[24];

                model.Submitted = true;
            }
            else
            {
                model.Surname = traderName;
                model.Error = await _localizationService.GetResourceAsync("App.Errors.NotSubmitted");
            }

            return model;
        }

        public async Task<ApdSubmissionSearchModel> PrepareApdSubmissionSearchModelAsync(ApdSubmissionSearchModel searchModel)
        {
            searchModel.Period = DateTime.Now.AddMonths(-1).ToUtcRelative();

            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.Payroll);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdSubmissionSearchModel>(nameof(ApdSubmissionSearchModel.SelectedKeys), FieldType.MultiSelectAll, options: traders)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdSubmissionSearchModel>(nameof(ApdSubmissionSearchModel.Period), FieldType.MonthDate, className: "col-12 md:col-6"),
                FieldConfig.Create<ApdSubmissionSearchModel>(nameof(ApdSubmissionSearchModel.Progress), FieldType.Text, _readonly: true, className: "col-12 md:col-6"),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public async Task<ApdSubmissionTableModel> PrepareApdSubmissionTableModelAsync(ApdSubmissionTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ApdSubmissionModel>(0, nameof(ApdSubmissionModel.Error)),
                ColumnConfig.Create<ApdSubmissionModel>(1, nameof(ApdSubmissionModel.Submitted), ColumnType.Checkbox),
                ColumnConfig.Create<ApdSubmissionModel>(2, nameof(ApdSubmissionModel.SubmissionNumber)),
                ColumnConfig.Create<ApdSubmissionModel>(3, nameof(ApdSubmissionModel.Type)),
                ColumnConfig.Create<ApdSubmissionModel>(4, nameof(ApdSubmissionModel.Ame)),
                ColumnConfig.Create<ApdSubmissionModel>(5, nameof(ApdSubmissionModel.Surname)),
                ColumnConfig.Create<ApdSubmissionModel>(6, nameof(ApdSubmissionModel.Vat)),
                ColumnConfig.Create<ApdSubmissionModel>(7, nameof(ApdSubmissionModel.Period), style: centerAlign),
                ColumnConfig.Create<ApdSubmissionModel>(8, nameof(ApdSubmissionModel.Amoe), hidden: true),
                ColumnConfig.Create<ApdSubmissionModel>(9, nameof(ApdSubmissionModel.TotalInsuranceDays), ColumnType.Number, width: 120, style: textAlign),
                ColumnConfig.Create<ApdSubmissionModel>(10, nameof(ApdSubmissionModel.TotalEarnings), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<ApdSubmissionModel>(11, nameof(ApdSubmissionModel.TotalContributions), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<ApdSubmissionModel>(12, nameof(ApdSubmissionModel.SubmissionDate), ColumnType.Text, width: 120, style: textAlign),
                ColumnConfig.Create<ApdSubmissionModel>(13, nameof(ApdSubmissionModel.Tpte), hidden: true)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ApdSubmissionModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}