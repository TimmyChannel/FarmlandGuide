using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void GenerateReportToExcel(string filePath, Enterprise enterprise)
        {
            try
            {
                _logger.Trace("Start generating report to excel file: {0}", filePath);

                // Получение данных для отчёта
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var enterpriseForReport = ctx.Enterprises
                    .Where(e => e.EnterpriseID == enterprise.EnterpriseID)
                    .Include(e => e.Employees)
                        .ThenInclude(e => e.Tasks)
                            .ThenInclude(t => t.ProductionProcess)
                                .ThenInclude(e => e.Tasks)
                                    .ThenInclude(t => t.Status)
                    .First();


                // Создание новой рабочей книги
                IWorkbook workbook = new XSSFWorkbook();

                ICellStyle headerStyle = workbook.CreateCellStyle();
                IFont headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);

                // Создание нового листа
                ISheet sheet = workbook.CreateSheet(enterpriseForReport.Name);

                // Объединённая ячейка для названия предприятия
                IRow titleRow = sheet.CreateRow(0);
                titleRow.HeightInPoints = 20; // Высота строки
                ICell titleCell = titleRow.CreateCell(0);
                titleCell.SetCellValue(enterprise.Name);
                titleCell.CellStyle = headerStyle;
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

                // Заголовок таблицы
                IRow headerRow = sheet.CreateRow(1);
                var headers = new[] { "ФИО", "Дата назначения", "Дата окончания срока", "Действие", "Статус" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = headerStyle;
                }


                int currentRow = 2;

                // Данные по задачам
                foreach (var employee in enterpriseForReport.Employees)
                {
                    foreach (var task in employee.Tasks)
                    {
                        if (task.ProductionProcess.EnterpriseID != enterpriseForReport.EnterpriseID) continue;
                        var row = sheet.CreateRow(currentRow++);

                        row.CreateCell(0).SetCellValue(employee.GetShortFIO());
                        row.CreateCell(1).SetCellValue(task.AssignmentDate.ToString("yyyy-MM-dd"));
                        row.CreateCell(2).SetCellValue(task.DueDate.ToString("yyyy-MM-dd"));
                        row.CreateCell(3).SetCellValue(task.ProductionProcess.Name);
                        row.CreateCell(4).SetCellValue(task.Status.Name);
                    }
                }
                currentRow++;
                // Подсчет статистики по задачам
                IRow summaryHeaderRow = sheet.CreateRow(currentRow++);
                var summaryHeaders = new[] { "Выполнил", "Сколько задач", "Успешно", "Провал/Просрочено" };
                for (int i = 0; i < summaryHeaders.Length; i++)
                {
                    ICell cell = summaryHeaderRow.CreateCell(i);
                    cell.SetCellValue(summaryHeaders[i]);
                    cell.CellStyle = headerStyle;
                }
                foreach (var employee in enterpriseForReport.Employees)
                {
                    var summaryRow = sheet.CreateRow(currentRow++);
                    summaryRow.CreateCell(0).SetCellValue(employee.GetShortFIO());
                    var tasks = employee.Tasks.Where(t => t.ProductionProcess.EnterpriseID == enterpriseForReport.EnterpriseID);
                    summaryRow.CreateCell(1).SetCellValue(tasks.Count());
                    int completed = tasks.Count(t => t.Status.Number == 1);
                    int failed = tasks.Count(t => t.Status.Number == 2);
                    summaryRow.CreateCell(2).SetCellValue(completed);
                    summaryRow.CreateCell(3).SetCellValue(failed);
                }

                // Авто-ресайз колонок
                for (int columnIndex = 0; columnIndex < headers.Length; columnIndex++)
                {
                    sheet.AutoSizeColumn(columnIndex);
                }

                // Сохранение документа Excel
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                _logger.Trace("Report successful saved to excel file: {0}", filePath);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне последний файл из папки /Logs/ \n Текст ошибки: {ex.Message}"));
            }
        }
    }

}

