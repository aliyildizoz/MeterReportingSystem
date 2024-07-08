using MeterService;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace ReportService.Helpers
{
    public class ExcelHelper
    {
        private static readonly string DirPath = @$"{Environment.CurrentDirectory}/wwwroot/reports";
        public static string CreateExcel(List<MeterReadingDto> meterReadingDtos, string serialNumber)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var filePath = @$"{DirPath}/{serialNumber}.xlsx";
            var file = new FileInfo(filePath);
            using ExcelPackage excel = new ExcelPackage(file);

            var workSheet = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == $"Meter Report({serialNumber})");
            if (workSheet == null) workSheet = excel.Workbook.Worksheets.Add($"Meter Report({serialNumber})");

            AddHeader(workSheet);

            Fill(workSheet, meterReadingDtos);

            if (File.Exists(filePath)) File.Delete(filePath);

            FileStream objFileStrm = File.Create(filePath);
            objFileStrm.Close();

            File.WriteAllBytes(filePath, excel.GetAsByteArray());
            excel.Dispose();

            return filePath;

        }

        private static void AddHeader(ExcelWorksheet workSheet)
        {
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            workSheet.Cells[1, 1].Value = "Number";
            workSheet.Cells[1, 2].Value = "Id";
            workSheet.Cells[1, 3].Value = "Serial Number";
            workSheet.Cells[1, 4].Value = "Reading Time";
            workSheet.Cells[1, 5].Value = "End Index";
            workSheet.Cells[1, 6].Value = "Voltage";
            workSheet.Cells[1, 7].Value = "Current";
        }

        private static void Fill(ExcelWorksheet workSheet, List<MeterReadingDto> meterReadingDtos, int recordIndex = 2)
        {

            foreach (var item in meterReadingDtos)
            {
                if (item != null)
                {
                    workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                    workSheet.Cells[recordIndex, 2].Value = item.Id;
                    workSheet.Cells[recordIndex, 3].Value = item.SerialNumber;
                    workSheet.Cells[recordIndex, 4].Value = item.ReadingTime.ToDateTime().ToString("d");
                    workSheet.Cells[recordIndex, 5].Value = item.EndIndex;
                    workSheet.Cells[recordIndex, 6].Value = item.Voltage;
                    workSheet.Cells[recordIndex, 7].Value = item.Current;
                    recordIndex++;
                }
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();
            workSheet.Column(6).AutoFit();
            workSheet.Column(7).AutoFit();
        }

    }
}
