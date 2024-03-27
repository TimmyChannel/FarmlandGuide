using FarmlandGuide.Helpers;
using FarmlandGuide.Models;
using FarmlandGuide.Models.Entities;
using NLog;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace FarmlandGuide
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public App()
        {
            var fileName = $"Logs\\{DateTime.Now:yyyyMMdd_HHmmss}.log";
            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Trace).WriteToDebug();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: fileName);
            });
            _logger.Debug("Debug start. File name: {0}", fileName);
            _logger.Trace("Database initial state configuration");
            using var ctx = new ApplicationDbContext();
            if (!ctx.Roles.Any())
            {
                _logger.Trace("Role table is empty. Addition new roles");
                ctx.Roles.Add(new Role() { Name = "Администратор" });
                ctx.Roles.Add(new Role() { Name = "Сотрудник" });
                ctx.SaveChangesAsync();
            }
            if (!ctx.Statuses.Any())
            {
                _logger.Trace("Status table is empty. New statuses addition");
                ctx.Statuses.Add(new Status("В работе", 0));
                ctx.Statuses.Add(new Status("Выполнено", 1));
                ctx.Statuses.Add(new Status("Провалено", 2));
                ctx.SaveChangesAsync();
            }
            if (!ctx.Enterprises.Any())
            {
                _logger.Trace("Enterprise table is empty. New enterprise addition");
                ctx.Enterprises.Add(new Enterprise("2fc4d315a2821ab76dd1c4931596c6ef", "2fc4d315a2821ab76dd1c4931596c6ef"));
                ctx.SaveChangesAsync();
            }
            if (!ctx.Employees.Any(e => e.EmployeeName == "admin"))
            {
                _logger.Trace("Admin user doesn`t exist. Admin addition");
                var salt = PasswordManager.GenerateSalt();
                var passwordHash = PasswordManager.HashPassword("AdministratorPassword34", salt);
                var employee = new Employee("Администратор БД", "", "", "", "", 0,
                    "", "admin", passwordHash, salt, 1, 1);
                ctx.Employees.Add(employee);
                ctx.SaveChanges();
            }
            _logger.Trace("Database initial state configurated");

        }
    }
}
