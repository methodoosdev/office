using App.Core;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Common.Pdf;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace App.Web.Controllers.Accounting
{
    public partial class MonthlyBCategoryBulletinController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IMonthlyBCategoryBulletinFactory _monthlyBCategoryBulletinFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IWorkContext _workContext;

        public MonthlyBCategoryBulletinController(ITraderService traderService,
            IMonthlyBCategoryBulletinFactory monthlyBCategoryBulletinFactory,
            ILocalizationService localizationService,
            IHtmlToPdfService htmlToPdfService,
            ITraderConnectionService traderConnectionService,
            IViewRenderService viewRenderService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _monthlyBCategoryBulletinFactory = monthlyBCategoryBulletinFactory;
            _localizationService = localizationService;
            _htmlToPdfService = htmlToPdfService;
            _traderConnectionService = traderConnectionService;
            _viewRenderService = viewRenderService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var searchModel = new MonthlyBCategoryBulletinSearchModel();
            searchModel.Period = DateTime.UtcNow;

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader != null)
            {
                searchModel.TraderId = trader.Id;
                var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
                if (connectionResult.Success)
                {
                    searchModel.ExpirationInventory = await _monthlyBCategoryBulletinFactory.PrepareExpirationAsync(connectionResult, searchModel.Period.Year, searchModel.Period.Month, "2_._1.%");
                    searchModel.ExpirationDepreciate = await _monthlyBCategoryBulletinFactory.PrepareExpirationAsync(connectionResult, searchModel.Period.Year - 1, searchModel.Period.Month, "66.%");
                }
            }

            //prepare searchModel
            searchModel = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinSearchModelAsync(searchModel);
            
            //prepare model
            var tableModel = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinTableModelAsync(new MonthlyBCategoryBulletinTableModel());

            //prepare model
            var remodelingCostsTable = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinRemodelingCostsQueryModelAsync(new MonthlyBCategoryBulletinRemodelingCostsQueryModel());
            var resultForm = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinResultFormModelAsync(new MonthlyBCategoryBulletinResultFormModel(), searchModel.Period.Year);

            return Json(new { searchModel, tableModel, remodelingCostsTable, resultForm });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] MonthlyBCategoryBulletinSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);
            
            var data = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinAsync(connectionResult, searchModel);

            return Json(data);
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> TraderChanged(int traderId, DateTime period)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(traderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var expirationInventory = await _monthlyBCategoryBulletinFactory.PrepareExpirationAsync(connectionResult, period.Year, period.Month, "2_._1.%");
            var expirationDepreciate = await _monthlyBCategoryBulletinFactory.PrepareExpirationAsync(connectionResult, period.Year - 1, period.Month, "66.%");

            return Json(new { expirationInventory, expirationDepreciate });
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf3([FromBody] MonthlyBCategoryPdfResult pdfModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(pdfModel.SearchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            IList<MonthlyBCategoryExpirationPdfModel> expirationList = new List<MonthlyBCategoryExpirationPdfModel>();

            //Αποθέματα έναρξης
            var beginningExpiration = await _monthlyBCategoryBulletinFactory.GetBeginningxpirationAsync(connectionResult, pdfModel.SearchModel);
            //Αποθέματα λήξης
            var endingExpiration = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.EndingInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = pdfModel.SearchModel.ExpirationInventory
            };
            //Διαφορά αποθεμάτων
            var expirationDifference = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.IncreaseInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = endingExpiration.Total - beginningExpiration.Total,
            };

            expirationList.Add(beginningExpiration);
            expirationList.Add(endingExpiration);
            expirationList.Add(expirationDifference);

            var groupList = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinPdfAsync(connectionResult, pdfModel.SearchModel);

            var currentYear = pdfModel.SearchModel.Period.Year;
            var period = currentYear.ToString();

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinModel.Title") + " - " + period,
                SubTitle = connectionResult.TraderName
            };

            var fields = new string[]
            {
                "ExpirationInventory", "NetProfitPeriod", "RemodelingCosts", "PreviousYearDamage",
                "TaxProfitPeriod", "TaxIncome", "HoldingTaxAdvance", "TaxAdvance",
                "PaymentPreviousYear", "TaxesFee", "AmountPayable", "TaxReturn"
            };

            var modelType = pdfModel.ResultModel.GetType();
            IList<MonthlyBCategoryPrintPdfResult> list2 = new List<MonthlyBCategoryPrintPdfResult>();
            foreach (var field in fields)
            {
                var description = await _localizationService.GetResourceAsync($"App.Models.MonthlyBCategoryBulletinResultFormModel.Fields.{field}");
                var value = modelType.GetProperty(field)?.GetValue(pdfModel.ResultModel, null) as decimal?;
                list2.Add(new MonthlyBCategoryPrintPdfResult { Type = description, Value = value.Value });
            }

            var template1 = "~/Views/Pdf/MonthlyBCategoryBulletin1.cshtml";
            var template2 = "~/Views/Pdf/MonthlyBCategoryBulletin2.cshtml";
            var template3 = "~/Views/Pdf/MonthlyBCategoryBulletin3.cshtml";


            byte[] bytes1 = await _htmlToPdfService.PrintListToPdf(groupList, template1, pdfItem, false);
            byte[] bytes2 = await _htmlToPdfService.PrintListToPdf(list2, template2, pdfItem, false);
            byte[] bytes3 = await _htmlToPdfService.PrintListToPdf(expirationList, template3, pdfItem, false);
            //byte[] bytes = await _htmlToPdfService.PrintToPdf((groupList, list2, expirationList), template, pdfItem, false);

            byte[] pdfBytes = GeneratePdfWithMultiplePages(new[] { bytes1, bytes2, bytes3 });

            return File(pdfBytes, MimeTypes.ApplicationPdf);
        }

        private byte[] GeneratePdfWithMultiplePages(byte[][] pageBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF document
                var document = new PdfDocument();

                foreach (var pageContent in pageBytes)
                {
                    // Add a new page to the document
                    var page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;

                    // Create an XGraphics object for drawing
                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        // Load the image from the byte array
                        using (var imageStream = new MemoryStream(pageContent))
                        {
                            var img = XImage.FromStream(() => new MemoryStream(pageContent));

                            // Calculate the scaling factor to fit the image to the page
                            double widthScale = page.Width / img.PixelWidth;
                            double heightScale = page.Height / img.PixelHeight;
                            double scale = Math.Min(widthScale, heightScale);

                            double width = img.PixelWidth * scale;
                            double height = img.PixelHeight * scale;

                            // Draw the image to fit the page
                            gfx.DrawImage(img, 0, 0, width, height);
                        }
                    }
                }

                // Save the document to the MemoryStream
                document.Save(memoryStream);

                // Return the PDF bytes
                return memoryStream.ToArray();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf1([FromBody] MonthlyBCategoryPdfResult pdfModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(pdfModel.SearchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            IList<MonthlyBCategoryExpirationPdfModel> expirationList = new List<MonthlyBCategoryExpirationPdfModel>();

            //Αποθέματα έναρξης
            var beginningExpiration = await _monthlyBCategoryBulletinFactory.GetBeginningxpirationAsync(connectionResult, pdfModel.SearchModel);
            //Αποθέματα λήξης
            var endingExpiration = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.EndingInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = pdfModel.SearchModel.ExpirationInventory
            };
            //Διαφορά αποθεμάτων
            var expirationDifference = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.IncreaseInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = endingExpiration.Total - beginningExpiration.Total,
            };

            expirationList.Add(beginningExpiration);
            expirationList.Add(endingExpiration);
            expirationList.Add(expirationDifference);

            var groupList = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinPdfAsync(connectionResult, pdfModel.SearchModel);

            var currentYear = pdfModel.SearchModel.Period.Year;
            var period = currentYear.ToString();

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinModel.Title") + " - " + period,
                SubTitle = connectionResult.TraderName
            };

            var fields = new string[]
            {
                "ExpirationInventory", "NetProfitPeriod", "RemodelingCosts", "PreviousYearDamage",
                "TaxProfitPeriod", "TaxIncome", "HoldingTaxAdvance", "TaxAdvance",
                "PaymentPreviousYear", "TaxesFee", "AmountPayable", "TaxReturn"
            };

            var modelType = pdfModel.ResultModel.GetType();
            IList<MonthlyBCategoryPrintPdfResult> list2 = new List<MonthlyBCategoryPrintPdfResult>();
            foreach (var field in fields)
            {
                var description = await _localizationService.GetResourceAsync($"App.Models.MonthlyBCategoryBulletinResultFormModel.Fields.{field}");
                var value = modelType.GetProperty(field)?.GetValue(pdfModel.ResultModel, null) as decimal?;
                list2.Add(new MonthlyBCategoryPrintPdfResult { Type = description, Value = value.Value });
            }

            var template1 = "~/Views/Pdf/MonthlyBCategoryBulletin1.cshtml";
            var template2 = "~/Views/Pdf/MonthlyBCategoryBulletin2.cshtml";
            var template3 = "~/Views/Pdf/MonthlyBCategoryBulletin3.cshtml";


            byte[] bytes1 = await _htmlToPdfService.PrintListToPdf(groupList, template1, pdfItem, false);
            byte[] bytes2 = await _htmlToPdfService.PrintListToPdf(list2, template2, pdfItem, false);
            byte[] bytes3 = await _htmlToPdfService.PrintListToPdf(expirationList, template3, pdfItem, false);
            //byte[] bytes = await _htmlToPdfService.PrintToPdf((groupList, list2, expirationList), template, pdfItem, false);

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var pdf1Entry = archive.CreateEntry("document1.pdf", CompressionLevel.Fastest);
                    using (var entryStream = pdf1Entry.Open())
                    {
                        entryStream.Write(bytes1, 0, bytes1.Length);
                    }

                    var pdf2Entry = archive.CreateEntry("document2.pdf", CompressionLevel.Fastest);
                    using (var entryStream = pdf2Entry.Open())
                    {
                        entryStream.Write(bytes2, 0, bytes2.Length);
                    }

                    // Add more files as needed
                    var pdf3Entry = archive.CreateEntry("document3.pdf", CompressionLevel.Fastest);
                    using (var entryStream = pdf3Entry.Open())
                    {
                        entryStream.Write(bytes3, 0, bytes3.Length);
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "documents.zip");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] MonthlyBCategoryPdfResult pdfModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(pdfModel.SearchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            IList<MonthlyBCategoryExpirationPdfModel> expirationList = new List<MonthlyBCategoryExpirationPdfModel>();

            //Αποθέματα έναρξης
            var beginningExpiration = await _monthlyBCategoryBulletinFactory.GetBeginningxpirationAsync(connectionResult, pdfModel.SearchModel);
            //Αποθέματα λήξης
            var endingExpiration = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.EndingInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = pdfModel.SearchModel.ExpirationInventory
            };
            //Διαφορά αποθεμάτων
            var expirationDifference = new MonthlyBCategoryExpirationPdfModel()
            {
                Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.IncreaseInventory"),
                Goods = 0,
                Materials = 0,
                Consumables = 0,
                SpareParts = 0,
                WarehouseOther = 0,
                Total = endingExpiration.Total - beginningExpiration.Total,
            };

            expirationList.Add(beginningExpiration);
            expirationList.Add(endingExpiration);
            expirationList.Add(expirationDifference);

            var groupList = await _monthlyBCategoryBulletinFactory.PrepareMonthlyBCategoryBulletinPdfAsync(connectionResult, pdfModel.SearchModel);

            var currentYear = pdfModel.SearchModel.Period.Year;
            var period = currentYear.ToString();

            var pdfItem = new HtmlPdfItem
            {
                Title = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinModel.Title") + " - " + period,
                SubTitle = connectionResult.TraderName
            };

            var fields = new string[]
            {
                "ExpirationInventory", "NetProfitPeriod", "RemodelingCosts", "PreviousYearDamage",
                "TaxProfitPeriod", "TaxIncome", "HoldingTaxAdvance", "TaxAdvance",
                "PaymentPreviousYear", "TaxesFee", "AmountPayable", "TaxReturn"
            };

            var modelType = pdfModel.ResultModel.GetType();
            IList<MonthlyBCategoryPrintPdfResult> list2 = new List<MonthlyBCategoryPrintPdfResult>();
            foreach (var field in fields)
            {
                var description = await _localizationService.GetResourceAsync($"App.Models.MonthlyBCategoryBulletinResultFormModel.Fields.{field}");
                var value = modelType.GetProperty(field)?.GetValue(pdfModel.ResultModel, null) as decimal?;
                list2.Add(new MonthlyBCategoryPrintPdfResult { Type = description, Value = value.Value });
            }

            var template = "~/Views/Pdf/MonthlyBCategoryBulletin.cshtml";

            byte[] bytes = await _htmlToPdfService.PrintToPdf((groupList, list2, expirationList), template, pdfItem, false);

            return File(bytes, MimeTypes.ApplicationPdf);
        }

    }
}