using App.Services.Localization;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Services.ExportImport
{
    public interface IImportFromExcelService
    {
        List<T> ImportExcel<T>(string excelFilePath, string sheetName, int startingColumn = 1);
    }
    public class ImportFromExcelService : IImportFromExcelService
    {
        private readonly ILocalizationService _localizationService;

        public ImportFromExcelService(
            ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public List<T> ImportExcel<T>(string excelFilePath, string sheetName, int startingColumn = 1)
        {
            List<T> list = new List<T>();
            Type  typeOfObject = typeof(T);
            using (IXLWorkbook workbook = new XLWorkbook(excelFilePath))
            {
                var worksheet = workbook.Worksheets.Where(x => x.Name == sheetName).First();
                var properties = typeOfObject.GetProperties();

                var columns = worksheet.FirstRow().Cells().Select((x, i) => new { Value = x.Value, Index = i + 1 }).ToList();
                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    T obj = (T)Activator.CreateInstance(typeOfObject);
                    var colIndex = startingColumn;
                    foreach (var prop in properties)
                    {
                        var value = row.Cell(colIndex).Value;
                        var type = prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(value, type), null);
                        colIndex++;
                    }

                    list.Add(obj);
                }
            }

            return list;
        }

        public List<T> ImportExcel2<T>(string excelFilePath, string sheetName)
        {
            List<T> list = new List<T>();
            Type typeOfObject = typeof(T);
            using (IXLWorkbook workbook = new XLWorkbook(excelFilePath))
            {
                var worksheet = workbook.Worksheets.Where(x => x.Name == sheetName).First();
                var properties = typeOfObject.GetProperties();

                var columns = worksheet.FirstRow().Cells().Select((x, i) => new { Value = x.Value, Index = i + 1 }).ToList();
                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    T obj = (T)Activator.CreateInstance(typeOfObject);
                    foreach (var prop in properties)
                    {
                        int colIndex = columns.SingleOrDefault(x =>
                        {
                            var value = x.Value.ToString();
                            var name = prop.Name.ToString();

                            if (value == name)
                                return true;

                            return false;
                        }).Index;
                        var value = row.Cell(colIndex).Value;
                        var type = prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(value, type), null);
                    }

                    list.Add(obj);
                }
            }

            return list;
        }
    }
}
