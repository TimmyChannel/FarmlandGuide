using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;

namespace FarmlandGuide.Models.Reports
{
    public class EnterpriseTaskReportGenerator
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void GenerateReportToExcel(string filePath, Entities.Enterprise enterprise)
        {
            try
            {
                Logger.Trace("Start generating report to excel file: {0}", filePath);

                using var ctx = new ApplicationDbContext();
                var enterpriseForReport = ctx.Enterprises
                    .Where(e => e.EnterpriseId == enterprise.EnterpriseId)
                    .Include(e => e.Employees)
                    .ThenInclude(e => e.Tasks)
                    .ThenInclude(t => t.ProductionProcess)
                    .ThenInclude(e => e.Tasks)
                    .ThenInclude(t => t.Status).Include(e => e.Employees)
                    .ThenInclude(employee => employee.Tasks).ThenInclude(task => task.Status)
                    .First();


                IWorkbook workbook = new XSSFWorkbook();

                ICellStyle headerStyle = workbook.CreateCellStyle();
                IFont headerFont = workbook.CreateFont();
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);

                ISheet sheet = workbook.CreateSheet(enterpriseForReport.Name);

                IRow titleRow = sheet.CreateRow(0);
                titleRow.HeightInPoints = 20; // Высота строки
                ICell titleCell = titleRow.CreateCell(0);
                titleCell.SetCellValue(enterprise.Name);
                titleCell.CellStyle = headerStyle;
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

                IRow headerRow = sheet.CreateRow(1);
                var headers = new[] { "ФИО", "Дата назначения", "Дата окончания срока", "Действие", "Статус" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = headerStyle;
                }


                int currentRow = 2;

                foreach (var employee in enterpriseForReport.Employees)
                {
                    foreach (var task in employee.Tasks)
                    {
                        if (task.ProductionProcess.EnterpriseId != enterpriseForReport.EnterpriseId) continue;
                        var row = sheet.CreateRow(currentRow++);

                        row.CreateCell(0).SetCellValue(employee.GetShortFio());
                        row.CreateCell(1).SetCellValue(task.AssignmentDate.ToString("yyyy-MM-dd"));
                        row.CreateCell(2).SetCellValue(task.DueDate.ToString("yyyy-MM-dd"));
                        row.CreateCell(3).SetCellValue(task.ProductionProcess.Name);
                        row.CreateCell(4).SetCellValue(task.Status.Name);
                    }
                }
                currentRow++;
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
                    summaryRow.CreateCell(0).SetCellValue(employee.GetShortFio());
                    var tasks = employee.Tasks.Where(t => t.ProductionProcess.EnterpriseId == enterpriseForReport.EnterpriseId);
                    summaryRow.CreateCell(1).SetCellValue(tasks.Count());
                    int completed = tasks.Count(t => t.Status.Number == 1);
                    int failed = tasks.Count(t => t.Status.Number == 2);
                    summaryRow.CreateCell(2).SetCellValue(completed);
                    summaryRow.CreateCell(3).SetCellValue(failed);
                }

                for (int columnIndex = 0; columnIndex < headers.Length; columnIndex++)
                {
                    sheet.AutoSizeColumn(columnIndex);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                Logger.Trace("Report successful saved to excel file: {0}", filePath);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
    }

}

