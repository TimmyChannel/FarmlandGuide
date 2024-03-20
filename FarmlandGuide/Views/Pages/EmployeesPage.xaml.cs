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

namespace FarmlandGuide.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    [ObservableObject]
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            SessionGrid.ItemsSource = TempModels.WorkSessions;
            EmployeesGrid.ItemsSource = TempModels.Employees;
        }
        [ObservableProperty]
        Employee _selectedEmployee;
        private void EmployeesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Employee)((DataGrid)sender).SelectedItem != null)
            {
                SelectedEmployee = (Employee)((DataGrid)sender).SelectedItem;
                Debug.WriteLine($"Selected employee name: {((Employee)((DataGrid)sender).SelectedItem).Name}");
            }
        }
    }
}
