using FarmlandGuide.Helpers;
using FarmlandGuide.Models;
using FarmlandGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Employee = FarmlandGuide.Models.Entities.Employee;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;
using Role = FarmlandGuide.Models.Entities.Role;

namespace FarmlandGuide
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public App()
        {
            var fileName = $"Logs\\{DateTime.Now:yyyyMMdd_HHmmss}.log";
            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Trace).WriteToDebug();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: fileName);
            });
            Logger.Debug("Debug start. File name: {0}", fileName);
            Logger.Trace("Database initial state configuration");
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (!ctx.Roles.Any())
            {
                Logger.Trace("Role table is empty. Addition new roles");
                ctx.Roles.Add(new Role() { Name = "Администратор" });
                ctx.Roles.Add(new Role() { Name = "Сотрудник" });
                ctx.SaveChangesAsync();
            }
            if (!ctx.Statuses.Any())
            {
                Logger.Trace("Status table is empty. New statuses addition");
                ctx.Statuses.Add(new Status("В работе", 0));
                ctx.Statuses.Add(new Status("Выполнено", 1));
                ctx.Statuses.Add(new Status("Провалено", 2));
                ctx.SaveChangesAsync();
            }
            if (!ctx.Enterprises.Any())
            {
                Logger.Trace("Enterprise table is empty. New enterprise addition");
                ctx.Enterprises.Add(new Enterprise("2fc4d315a2821ab76dd1c4931596c6ef", "2fc4d315a2821ab76dd1c4931596c6ef"));
                ctx.SaveChangesAsync();
            }
            if (!ctx.Employees.Any(e => e.EmployeeName == "admin"))
            {
                Logger.Trace("Admin user doesn`t exist. Admin addition");
                var salt = PasswordManager.GenerateSalt();
                var passwordHash = PasswordManager.HashPassword("AdministratorPassword34", salt);
                var employee = new Employee("Администратор БД", "", "", "", "", 0,
                    "", "admin", passwordHash, salt, 1, 1);
                ctx.Employees.Add(employee);
                ctx.SaveChanges();
            }
            Logger.Trace("Database initial state configurated");

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
