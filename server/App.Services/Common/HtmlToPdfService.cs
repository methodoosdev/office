using App.Services.Common.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace App.Services.Common
{
    public interface IHtmlToPdfService
    {
        Task<byte[]> PrintListToPdf<T>(IList<T> list, string template, HtmlPdfItem item, bool portrait = true);
        Task PrintListToPdf<T>(Stream stream, IList<T> list, string template, HtmlPdfItem item, bool portrait = true);
        Task<byte[]> PrintToPdf<T>(T model, string template, HtmlPdfItem item, bool portrait = true);
        Task PrintToPdf<T>(Stream stream, T model, string template, HtmlPdfItem item, bool portrait = true);
    }
    
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly IViewRenderService _viewRenderService;

        public HtmlToPdfService(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        public async Task<byte[]> PrintListToPdf<T>(IList<T> list, string template, HtmlPdfItem item, bool portrait = true)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                await PrintListToPdf(stream, list, template, item, portrait);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public async Task PrintListToPdf<T>(Stream stream, IList<T> list, string template, HtmlPdfItem item, bool portrait = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            var html = await _viewRenderService.RenderToStringAsync<(IList<T>, HtmlPdfItem)>(template, new(list, item));

            //TextReader sr = new StringReader(html);
            //using var doc = Scryber.Components.Document.ParseDocument(sr, Scryber.ParseSourceType.DynamicContent);
            //doc.SaveAsPDF(stream);

            using (var reader = new StringReader(html))
            {
                using var doc = Scryber.Components.Document.ParseDocument(reader, Scryber.ParseSourceType.DynamicContent);
                
                var page = doc.Pages[0] as Scryber.Components.Page;
                page.Padding = 10;
                page.PaperSize = Scryber.PaperSize.A4;
                page.PaperOrientation = portrait ? Scryber.PaperOrientation.Portrait : Scryber.PaperOrientation.Landscape;

                doc.SaveAsPDF(stream);
            }
        }

        public async Task<byte[]> PrintToPdf<T>(T model, string template, HtmlPdfItem item, bool portrait = true)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                await PrintToPdf(stream, model, template, item, portrait);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public async Task PrintToPdf<T>(Stream stream, T model, string template, HtmlPdfItem item, bool portrait = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var html = await _viewRenderService.RenderToStringAsync<(T, HtmlPdfItem)>(template, new(model, item));

            //TextReader sr = new StringReader(html);
            //using var doc = Scryber.Components.Document.ParseDocument(sr, Scryber.ParseSourceType.DynamicContent);
            //doc.SaveAsPDF(stream);

            using (var reader = new StringReader(html))
            {
                using var doc = Scryber.Components.Document.ParseDocument(reader, Scryber.ParseSourceType.DynamicContent);

                var page = doc.Pages[0] as Scryber.Components.Page;
                page.Padding = 10;
                page.PaperSize = Scryber.PaperSize.A4;
                page.PaperOrientation = portrait ? Scryber.PaperOrientation.Portrait : Scryber.PaperOrientation.Landscape;

                doc.SaveAsPDF(stream);
            }
        }
    }
}