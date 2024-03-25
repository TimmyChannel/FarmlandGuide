using FarmlandGuide.Models;
using FarmlandGuide.Models.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
        }
    }
}
