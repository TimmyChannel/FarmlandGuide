using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models.Reports
{
    public class EnterpriseTaskReportGenerator
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void GenerateReportToExcel(string filePath, Enterprise enterprise)
        {
            _logger.Trace("Start generating report to excel file: {0}", filePath);
            // Создание новой рабочей книги
            IWorkbook workbook = new XSSFWorkbook();

            // Создание нового листа
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // Добавление данных в ячейки
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("Привет");
            row.CreateCell(1).SetCellValue("Мир");

            // Сохранение документа Excel
            using FileStream fileStream = new(filePath, FileMode.Create);
            workbook.Write(fileStream, false);
            _logger.Trace("Report successful saved to excel file: {0}", filePath);

        }

    }
}
