using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;

namespace App.Automation
{
    public static class iTextSharpExtensions
    {
        public static void PDFtoImage2(string source)
        {
            using (PdfReader reader = new PdfReader(source))
            {
                //var rect = reader.GetPageSize(1);
                PdfReaderContentParser parser = new PdfReaderContentParser(reader);
                ImageWithTitleRenderListener listener = new ImageWithTitleRenderListener(@"f2-{0:D3}.{1}");
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    parser.ProcessContent(i, listener);
                }
            }
        }

        //public static void PDFtoImage(string source, string target)
        //{
        //    FindImages(source, target);
        //}

        //private static void FindImages(string source, string target)
        //{
        //    using (var pdf = new PdfReader(source))
        //    {
        //        for (int pageNumber = 1, imageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++, imageNumber = 1)
        //        {
        //            FindPageImages(pdf.GetPageN(pageNumber), obj =>
        //            {
        //                if (obj == null)
        //                    return;

        //                var pdfObj = pdf.GetPdfObject(((PRIndirectReference)obj).Number);
        //                if (pdfObj == null || !pdfObj.IsStream())
        //                    return;

        //                var stream = (PdfStream)pdfObj;
        //                var subtype = stream.Get(PdfName.SUBTYPE);

        //                if (subtype == null || !subtype.Equals(PdfName.IMAGE))
        //                    return;

        //                var imageObj = new PdfImageObject((PRStream)stream);

        //                var path = System.IO.Path.Combine(target, $"Image {pageNumber} - {imageNumber++}.{imageObj.GetFileType()}");
        //                using (var image = imageObj.GetDrawingImage())
        //                {
        //                    image.Save(filename: path);
        //                }
        //            });
        //        }
        //    }
        //}

        private static void FindPageImages(PdfDictionary section, Action<PdfObject> callback)
        {
            var resources = (PdfDictionary)PdfReader.GetPdfObject(section.Get(PdfName.RESOURCES));
            var objs = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
            if (objs == null)
                return;

            foreach (var key in objs.Keys)
            {
                var obj = objs.Get(key);
                if (!obj.IsIndirect())
                    continue;

                var pdfObj = (PdfDictionary)PdfReader.GetPdfObject(obj);
                var type = (PdfName)PdfReader.GetPdfObject(pdfObj.Get(PdfName.SUBTYPE));

                if (PdfName.IMAGE.Equals(type))
                    callback(obj);
                else if (PdfName.FORM.Equals(type) || PdfName.GROUP.Equals(type))
                    FindPageImages(pdfObj, callback);
            }
        }

    }
    public class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
    {
        private string fieldName;
        public string FieldName => fieldName;
        public MyLocationTextExtractionStrategy(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }
    public class MyMultiFilteredRenderListener : MultiFilteredRenderListener
    {
        static Rectangle GetRectangle(TextRenderInfo textRenderInfo)
        {
            LineSegment descentLine = textRenderInfo.GetDescentLine();
            LineSegment ascentLine = textRenderInfo.GetAscentLine();
            float x0 = descentLine.GetStartPoint()[0];
            float x1 = descentLine.GetEndPoint()[0];
            float y0 = descentLine.GetStartPoint()[1];
            float y1 = ascentLine.GetEndPoint()[1];
            return new Rectangle(x0, y0, x1, y1);
        }

        public override void RenderText(TextRenderInfo renderInfo)
        {
            base.RenderText(renderInfo);

            var str = renderInfo.GetText();
            if (str.Trim() == "137.904,99")
            {
                var dd2 = GetRectangle(renderInfo);
            }

        }
    }

    internal class ImageWithTitleRenderListener : IRenderListener
    {
        int imageNumber = 0;
        string format;
        bool expectingTitle = false;
        int index = 0;
        IList<(int index, string value)> text = new List<(int index, string value)>();

        public ImageWithTitleRenderListener(string format)
        {
            this.format = format;
        }

        public void BeginTextBlock()
        { }

        public void EndTextBlock()
        {
            var test = text.ToString();
        }

        static Rectangle GetRectangle(TextRenderInfo textRenderInfo)
        {
            LineSegment descentLine = textRenderInfo.GetDescentLine();
            LineSegment ascentLine = textRenderInfo.GetAscentLine();
            float x0 = descentLine.GetStartPoint()[0];
            float x1 = descentLine.GetEndPoint()[0];
            float y0 = descentLine.GetStartPoint()[1];
            float y1 = ascentLine.GetEndPoint()[1];
            return new Rectangle(x0, y0, x1, y1);
        }
        public void RenderText(TextRenderInfo renderInfo)
        {
            var str = renderInfo.GetText();
            if (str.Trim() == "32.185,73")
            {
                var dd2 = GetRectangle(renderInfo);

                //Get the bounding box for the chunk of text
                var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
                var topRight = renderInfo.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new Rectangle(
                                                        bottomLeft[Vector.I1],
                                                        bottomLeft[Vector.I2],
                                                        topRight[Vector.I1],
                                                        topRight[Vector.I2]
                                                        );
            }

            if (str.Trim() == "7.724,58")
            {
                //Get the bounding box for the chunk of text
                var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
                var topRight = renderInfo.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new Rectangle(
                                                        bottomLeft[Vector.I1],
                                                        bottomLeft[Vector.I2],
                                                        topRight[Vector.I1],
                                                        topRight[Vector.I2]
                                                        );
            }

            if (str.Trim() == "13.407,71")
            {
                var dd2 = GetRectangle(renderInfo);
            }

            if (str.Trim() == "2.253,84")
            {
                var dd2 = GetRectangle(renderInfo);
            }

            if (str.Trim() == "34.439,57")
            {
                var dd2 = GetRectangle(renderInfo);
            }

            if (str.Trim() == "11.452,76")
            {
                var dd2 = GetRectangle(renderInfo);
            }

            text.Add((index, str));
            index++;

            if (expectingTitle)
            {
                expectingTitle = false;
                System.IO.File.WriteAllText(string.Format(format, imageNumber, "txt"), renderInfo.GetText());
            }
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            imageNumber++;
            expectingTitle = true;

            PdfImageObject imageObject = renderInfo.GetImage();

            if (imageObject == null)
            {
                Console.WriteLine("Image {0} could not be read.", imageNumber);
            }
            else
            {
                var type = imageObject.GetFileType();
                //System.IO.File.WriteAllBytes(string.Format(format, imageNumber, type), imageObject.GetImageAsBytes());
            }
        }
    }
}