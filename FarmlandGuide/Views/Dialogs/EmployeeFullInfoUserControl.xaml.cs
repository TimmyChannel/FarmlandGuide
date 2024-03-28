using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для EmployeeFullInfoUserControl.xaml
    /// </summary>
    [ObservableObject]
    public partial class EmployeeFullInfoUserControl : UserControl
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty EmployeeProperty = DependencyProperty.Register(
            "Employee",
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
        [ObservableProperty] private int _tasksCountFromLastMonthSucces;
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
                _logger.Trace("Full info user control loaded. Employee: {0}", Employee.ToString());
                using var ctx = new ApplicationDbContext();
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                _enterprise = ctx.Enterprises.Find(Employee.EnterpriseID);
                //_workSessions = new(ctx.WorkSessions.Where(ws => ws.EmployeeID == Employee.EmployeeID).ToList());
                _tasks = new ObservableCollection<Task>(ctx.Tasks.Where(t => t.EmployeeID == Employee.EmployeeID).Include(t => t.ProductionProcess).Include(t => t.Status).ToList());

                Debug.WriteLine($"Employee {Employee}");
                FIO = Employee.ToString();
                Position = Employee.Position;
                WorkSchedule = Employee.WorkSchedule;
                FixedSalary = Employee.Salary;
                ResidentialAddress = Employee.ResidentialAddress;
                EnterpriseName = _enterprise.Name;
                CalculateSalaryFromTasks();
                _logger.Trace("Full info about employee: {0} loaded", Employee.ToString());

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне последний файл из папки /Logs/ \n Текст ошибки: {ex.Message}"));
            }

        }
        private void CalculateSalaryFromTasks()
        {
            try
            {
                _logger.Trace("Start salary and tasks calculation");
                TasksCountFromLastMonthFailed = _tasks.Where(t => t.DueDate >= DateTime.Now.AddMonths(-1)).Count(t => t.Status.Number == 2);
                TasksCountFromLastMonthSucces = _tasks.Where(t => t.DueDate >= DateTime.Now.AddMonths(-1)).Count(t => t.Status.Number == 1);
                TasksCountFromLastMonthInWork = _tasks.Count(t => t.Status.Number == 0);
                CalculatedSalary = _tasks.Where(t => t.Status.Number == 1 && t.DueDate >= DateTime.Now.AddMonths(-1)).Sum(t => t.ProductionProcess.Cost);
                _logger.Trace("Complete salary and tasks calculation.\n" +
                    "Task count: failed - {0}, success - {1}, in work - {2}. Calculated salary - {3}", TasksCountFromLastMonthFailed, TasksCountFromLastMonthSucces, TasksCountFromLastMonthInWork, CalculatedSalary);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне последний файл из папки /Logs/ \n Текст ошибки: {ex.Message}"));
            }

        }
    }
}
