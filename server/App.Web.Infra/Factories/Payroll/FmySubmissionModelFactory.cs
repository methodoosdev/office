using App.Core;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Payroll;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Payroll;
using App.Services;
using App.Services.Localization;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IFmySubmissionModelFactory
    {
        void PrepareCompareFmy(FmySubmissionModel value1, FmySubmissionModel value2);
        Task<FmySubmissionSearchModel> PrepareFmySubmissionSearchModelAsync(FmySubmissionSearchModel searchModel);
        Task<IList<FmySubmissionModel>> GetFmyFromHyperMAsync(string connection, int monthFrom, int monthTo, int year, int companyId, string traderName);
        IList<FmySubmissionModel> ExtractFmySubmissionModel(IList<string> list, string traderName);
        Task<FmySubmissionTableModel> PrepareFmySubmissionTableModelAsync(FmySubmissionTableModel tableModel);
    }

    public partial class FmySubmissionModelFactory : IFmySubmissionModelFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IAppDataProvider _dataProvider;

        public FmySubmissionModelFactory(
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IAppDataProvider dataProvider)
        {
            _localizationService = localizationService;
            _modelFactoryService = modelFactoryService;
            _dataProvider = dataProvider;
        }

        public void PrepareCompareFmy(FmySubmissionModel value1, FmySubmissionModel value2)
        {
            if (value1 == null || value2 == null)
                return;

            var compare = false;
            var stampTotal = value2.StampTotal.HasValue ? value2.StampTotal.Value : 0m;
            var compensation = value2.Compensation.HasValue ? value2.Compensation.Value : 0m;


            if (value1.GrossEarnings.Equals(value2.GrossEarnings) && value1.TaxAmount.Equals(value2.TaxAmount) &&
                value1.Contribution.Equals(value2.Contribution) && value1.Stamp.Equals(value2.Stamp) &&
                value1.StampTotal.Equals(stampTotal) && value1.Compensation.Equals(compensation))
                compare = true;

            value2.StampTotal = stampTotal;
            value2.Compensation = compensation;
            value2.Check = compare;
        }

        public async Task<FmySubmissionSearchModel> PrepareFmySubmissionSearchModelAsync(FmySubmissionSearchModel searchModel)
        {
            var now = DateTime.Now.AddMonths(-2).ToUtcRelative();
            searchModel.From = now;
            searchModel.To = now;

            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.Payroll);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FmySubmissionSearchModel>(nameof(FmySubmissionSearchModel.SelectedKeys), FieldType.MultiSelectAll, options: traders)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FmySubmissionSearchModel>(nameof(FmySubmissionSearchModel.From), FieldType.MonthDate, className: "col-12 md:col-3"),
                FieldConfig.Create<FmySubmissionSearchModel>(nameof(FmySubmissionSearchModel.To), FieldType.MonthDate, className: "col-12 md:col-3"),
                FieldConfig.Create<FmySubmissionSearchModel>(nameof(FmySubmissionSearchModel.Progress), FieldType.ProgressBar, className: "col-12 md:col-6"),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public async Task<IList<FmySubmissionModel>> GetFmyFromHyperMAsync(string connection, int monthFrom, int monthTo, int year, int companyId, string traderName)
        {
            var months = new List<int>();
            for (var i = monthFrom; i <= monthTo; i++)
                months.Add(i);

            var list = new List<FmySubmissionModel>();
            foreach (var month in months)
            {
                var pMonth = new LinqToDB.Data.DataParameter("pMonth", month);
                var pYear = new LinqToDB.Data.DataParameter("pYear", year);
                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);

                var model = (await _dataProvider.QueryAsync<FmySubmissionModel>(connection, FmySubmissionQuery.Get, pMonth, pYear, pCompanyId)).FirstOrDefault();

                if (model == null)
                    model = new FmySubmissionModel();

                model.Period = $"{month}/{year}";
                model.Surname = traderName;
                model.HyperMItem = true;

                list.Add(model);
            }

            return list;
        }

        public IList<FmySubmissionModel> ExtractFmySubmissionModel(IList<string> items, string traderName)
        {
            var models = new List<FmySubmissionModel>();

            foreach (var item in items)
            {
                var lines = item.Split('\n');
                var period = lines[1].Split('/');

                var fmyLines = lines.Where(x => x.StartsWith("Μήνας")).ToList();
                var fmyLine0 = fmyLines[0].Split(' ');
                //var fmyLine1 = fmyLines[1].Split(' ');
                var fmyLine2 = fmyLines[2].Split(' ');
                //var fmyLine3 = fmyLines[3].Split(' ');
                //var fmyLine4 = fmyLines[4].Split(' ');
                var fmyLine5 = fmyLines[5].Split(' ');

                // 0 = "ΕΙΣΟΔΗΜΑ ΑΠΟ ΜΙΣΘΩΤΗ ΕΡΓΑΣΙΑ";
                // 1 = "ΕΙΣΟΔΗΜΑ ΑΠΟ ΣΥΝΤΑΞΕΙΣ";
                // 2 = "ΑΜΟΙΒΕΣ ΠΟΥ ΥΠΟΚΕΙΝΤΑΙ ΣΕ ΤΕΛΟΣ ΧΑΡΤΟΣΗΜΟΥ ΠΛΕΟΝ ΕΙΣΦΟΡΑΣ ΥΠΕΡ ΟΓΑ";
                // 3 = "ΠΑΡΑΚΡΑΤΗΣΗ ΦΟΡΟΥ ΣΕ ΠΕΡΙΟΔΙΚΑ ΚΑΤΑΒΑΛΛΟΜΕΝΟ ΑΣΦΑΛΙΣΜΑ ΟΜΑΔΙΚΩΝ ΑΣΦΑΛΙΣΤΗΡΙΩΝ";
                // 4 = "ΑΣΦΑΛΙΣΜΑ ΟΜΑΔΙΚΩΝ ΑΣΦΑΛΙΣΤΗΡΙΩΝ ΜΕ ΕΦΑΠΑΞ ΚΑΤΑΒΟΛΗ";
                // 5 = "ΑΠΟΖΗΜΙΩΣΗ ΑΠΟΛΥΟΜΕΝΩΝ";

                var model = new FmySubmissionModel
                {
                    Surname = traderName,
                    SubmissionDate = lines[3],
                    SubmissionType = lines[4].Split(':')[2].Trim(),
                    Period = $"{period[1]}/{period[2].Split(' ')[0]}",
                    GrossEarnings = fmyLine0[2].ToDecimal() + fmyLine5[2].ToDecimal(),
                    TaxAmount = fmyLine0[3].ToDecimal(),
                    Contribution = fmyLine0[4].ToDecimal(),
                    Stamp = fmyLine2[5].ToDecimal(),
                    StampTotal = fmyLine2[2].ToDecimal(),
                    Stamp1 = fmyLine2[3].ToDecimal(),
                    StampOga = fmyLine2[4].ToDecimal(),
                    Compensation = fmyLine5[2].ToDecimal(),
                    Check = true
                };

                models.Add(model);
            }

            if (items.Count == 0)
            {
                var model = new FmySubmissionModel { Surname = traderName };

                models.Add(model);
            }

            return models;
        }

        public async Task<FmySubmissionTableModel> PrepareFmySubmissionTableModelAsync(FmySubmissionTableModel tableModel)
        {
            var textAlignRight = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<FmySubmissionModel>(0, nameof(FmySubmissionModel.Surname)),
                ColumnConfig.Create<FmySubmissionModel>(1, nameof(FmySubmissionModel.Check), ColumnType.Checkbox),
                ColumnConfig.Create<FmySubmissionModel>(2, nameof(FmySubmissionModel.SubmissionDate)),
                ColumnConfig.Create<FmySubmissionModel>(3, nameof(FmySubmissionModel.SubmissionType)),
                ColumnConfig.Create<FmySubmissionModel>(4, nameof(FmySubmissionModel.Period)),
                ColumnConfig.Create<FmySubmissionModel>(5, nameof(FmySubmissionModel.GrossEarnings), ColumnType.Decimal, style: textAlignRight),
                ColumnConfig.Create<FmySubmissionModel>(6, nameof(FmySubmissionModel.TaxAmount), ColumnType.Decimal, style: textAlignRight),
                ColumnConfig.Create<FmySubmissionModel>(7, nameof(FmySubmissionModel.Contribution), ColumnType.Decimal, style: textAlignRight),
                ColumnConfig.Create<FmySubmissionModel>(8, nameof(FmySubmissionModel.Stamp), ColumnType.Decimal, style: textAlignRight),
                //ColumnConfig.Create<FmySubmissionModel>(8, nameof(FmySubmissionModel.Stamp1), ColumnType.Decimal, style: textAlignRight),
                //ColumnConfig.Create<FmySubmissionModel>(8, nameof(FmySubmissionModel.StampOga), ColumnType.Decimal, style: textAlignRight)
                ColumnConfig.Create<FmySubmissionModel>(9, nameof(FmySubmissionModel.StampTotal), ColumnType.Decimal, style: textAlignRight),
                ColumnConfig.Create<FmySubmissionModel>(11, nameof(FmySubmissionModel.Compensation), ColumnType.Decimal, style: textAlignRight)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.FmySubmissionModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}