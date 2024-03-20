using CommunityToolkit.Mvvm.ComponentModel;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
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

namespace FarmlandGuide.Views.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для EmployeeFullInfoUserControl.xaml
    /// </summary>
    [ObservableObject]
    public partial class EmployeeFullInfoUserControl : UserControl
    {
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
        [ObservableProperty]
        string _fIO;
        [ObservableProperty]
        string _position;

        private void FullInfoUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Employee {Employee}");
            FIO = Employee.Name;
            Position = Employee.Position;

        }
    }
}
