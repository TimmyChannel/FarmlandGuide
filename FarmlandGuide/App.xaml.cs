using FarmlandGuide.Helpers;
using FarmlandGuide.Models;
using FarmlandGuide.Models.Entities;
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
        public App()
        {
            using var ctx = new ApplicationDbContext();
            if (!ctx.Roles.Any())
            {
                ctx.Roles.Add(new Role() { Name = "Администратор" });
                ctx.Roles.Add(new Role() { Name = "Сотрудник" });
                ctx.SaveChangesAsync();
            }
            if (!ctx.Statuses.Any())
            {
                ctx.Statuses.Add(new Status("В работе", 0));
                ctx.Statuses.Add(new Status("Выполнено", 1));
                ctx.Statuses.Add(new Status("Провалено", 2));
                ctx.SaveChangesAsync();
            }  
            if (!ctx.Enterprises.Any())
            {
                ctx.Enterprises.Add(new Enterprise("2fc4d315a2821ab76dd1c4931596c6ef", "2fc4d315a2821ab76dd1c4931596c6ef"));
                ctx.SaveChangesAsync();
            }
            if (!ctx.Employees.Any(e => e.EmployeeName == "admin"))
            {
                var salt = PasswordManager.GenerateSalt();
                var passwordHash = PasswordManager.HashPassword("AdministratorPassword34", salt);
                var employee = new Employee("Администратор БД", "", "", "", "", 0,
                    "", "admin", passwordHash, salt, 1, 1);
                ctx.Employees.Add(employee);
                ctx.SaveChanges();
            }

        }
    }
}
