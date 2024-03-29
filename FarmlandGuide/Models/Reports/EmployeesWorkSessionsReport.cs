using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace FarmlandGuide.Models.Reports
{
    public class EmployeesWorkSessionsReportGenerator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void GenerateReportToExcel(string filePath)
        {
            try
            {
                Logger.Trace("Start generating report to excel file: {0}", filePath);

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Рабочие сессии");

                using var ctx = new ApplicationDbContext();
                var employees = ctx.Employees.Include(e => e.WorkSessions);

                int rownum = 1;
                foreach (var employee in employees)
                {
                    if (employee.EmployeeName == "admin") continue;
                    // Добавляем информацию о сотруднике
                    IRow titleRow = sheet.CreateRow(rownum);
                    titleRow.HeightInPoints = 20; // Высота строки
                    titleRow.CreateCell(0).SetCellValue(employee.ToString());
                    sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum, 0, 3));
                    int initialRow = rownum++;
                    foreach (var session in employee.WorkSessions)
                    {
                        IRow row = sheet.CreateRow(rownum++);
                        FillRowWithSessionInfo(row, session);
                    }

                    // Добавляем итоги
                    if (initialRow < rownum)
                    {
                        AddSummaryRows(sheet, rownum, employee.WorkSessions.ToList());
                        rownum += 4; // Пропускаем строки для подытогов и пустую строку
                    }
                }
                for (int columnIndex = 0; columnIndex < 4; columnIndex++)
                {
                    sheet.AutoSizeColumn(columnIndex);
                }
                // Сохраняем файл
                SaveWorkbook(workbook, filePath);
                Logger.Trace("Report successful saved to excel file: {0}", filePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }


        private static void FillRowWithSessionInfo(IRow row, WorkSession session)
        {
            row.CreateCell(0).SetCellValue(session.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            row.CreateCell(1).SetCellValue(session.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            row.CreateCell(2).SetCellValue(session.Type);
        }


        private static void SaveWorkbook(IWorkbook workbook, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            workbook.Write(fs);
        }

        private static void AddSummaryRows(ISheet sheet, int startRow, List<WorkSession> sessions)
        {
            var sessionTypes = new Dictionary<string, (int, double)> { { "Работа", (0, 0) }, { "Отпуск", (0, 0) }, { "Выходной", (0, 0) } };
            IRow headerRow = sheet.CreateRow(startRow++);
            headerRow.CreateCell(1).SetCellValue("Число сессий");
            headerRow.CreateCell(2).SetCellValue("Число дней");

            foreach (var session in sessions)
            {
                string type = ClassifySession(session.Type);
                if (sessionTypes.ContainsKey(type))
                {
                    var (sessionCount, dayCount) = sessionTypes[type];
                    sessionCount++;
                    dayCount += (session.EndDateTime - session.StartDateTime).TotalDays;
                    sessionTypes[type] = (sessionCount, dayCount);
                }
            }

            int rownum = startRow;
            foreach (var type in sessionTypes)
            {
                IRow row = sheet.CreateRow(rownum++);
                row.CreateCell(0).SetCellValue(type.Key);
                row.CreateCell(1).SetCellValue(type.Value.Item1);
                row.CreateCell(2).SetCellValue(Math.Round(type.Value.Item2, MidpointRounding.AwayFromZero));
            }
        }

        private static string ClassifySession(string description)
        {
            if (Regex.IsMatch(description, @"\bработа\b", RegexOptions.IgnoreCase))
            {
                return "Работа";
            }

            if (Regex.IsMatch(description, @"\bотпуск\b", RegexOptions.IgnoreCase))
            {
                return "Отпуск";
            }

            if (Regex.IsMatch(description, @"\bвыходной\b", RegexOptions.IgnoreCase))
            {
                return "Выходной";
            }
            return "Другое";
        }
    }
}
