using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NLog;
using NLog.Targets;
using Employee = FarmlandGuide.Models.Entities.Employee;
using Enterprise = FarmlandGuide.Models.Entities.Enterprise;
using Task = FarmlandGuide.Models.Entities.Task;

namespace FarmlandGuide.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для EmployeeFullInfoUserControl.xaml
    /// </summary>
    [ObservableObject]
    public partial class EmployeeFullInfoUserControl : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty EmployeeProperty = DependencyProperty.Register(
            nameof(Employee),
            typeof(Employee),
            typeof(EmployeeFullInfoUserControl));
        public EmployeeFullInfoUserControl()
        {
            InitializeComponent();
        }
        public Employee Employee
        {
            get => (Employee)GetValue(EmployeeProperty);
            set => SetValue(EmployeeProperty, value);
        }
        [ObservableProperty] private string _fIO;
        [ObservableProperty] private string _position;
        [ObservableProperty] private string _workSchedule;
        [ObservableProperty] private decimal _fixedSalary;
        [ObservableProperty] private string _residentialAddress;
        [ObservableProperty] private string _enterpriseName;
        [ObservableProperty] private int _tasksCountFromLastMonthSuccess;
        [ObservableProperty] private int _tasksCountFromLastMonthInWork;
        [ObservableProperty] private int _tasksCountFromLastMonthFailed;
        [ObservableProperty] private decimal _calculatedSalary;

        private Enterprise _enterprise;
        //ObservableCollection<WorkSession> _workSessions;
        private ObservableCollection<Task> _tasks;

        private void FullInfoUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Trace("Full info user control loaded. Employee: {0}", Employee.ToString());
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                _enterprise = ctx.Enterprises.Find(Employee.EnterpriseId);
                _tasks = new ObservableCollection<Task>(ctx.Tasks.Where(t => t.EmployeeId == Employee.EmployeeId).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());

                Debug.WriteLine($"Employee {Employee}");
                FIO = Employee.ToString();
                Position = Employee.Position;
                WorkSchedule = Employee.WorkSchedule;
                FixedSalary = Employee.Salary;
                ResidentialAddress = Employee.ResidentialAddress;
                EnterpriseName = _enterprise.Name;
                CalculateSalaryFromTasks();
                Logger.Trace("Full info about employee: {0} loaded", Employee.ToString());

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }

        }
        private void CalculateSalaryFromTasks()
        {
            try
            {
                Logger.Trace("Start salary and tasks calculation");
                TasksCountFromLastMonthFailed = _tasks.Where(t => t.DueDate >= DateTime.Now.AddMonths(-1)).Count(t => t.Status.Number == 2);
                TasksCountFromLastMonthSuccess = _tasks.Where(t => t.DueDate >= DateTime.Now.AddMonths(-1)).Count(t => t.Status.Number == 1);
                TasksCountFromLastMonthInWork = _tasks.Count(t => t.Status.Number == 0);
                CalculatedSalary = _tasks.Where(t => t.Status.Number == 1 && t.DueDate >= DateTime.Now.AddMonths(-1)).Sum(t => t.ProductionProcess.Cost);
                Logger.Trace("Complete salary and tasks calculation.\n" +
                    "Task count: failed - {0}, success - {1}, in work - {2}. Calculated salary - {3}", TasksCountFromLastMonthFailed, TasksCountFromLastMonthSuccess, TasksCountFromLastMonthInWork, CalculatedSalary);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }

        }
    }
}
