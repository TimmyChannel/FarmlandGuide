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
    /// Логика взаимодействия для WorkSessionAddEditDialog.xaml
    /// </summary>
    public partial class WorkSessionAddEditDialog : Page
    {
        public WorkSessionAddEditDialog()
        {
            InitializeComponent();
        }
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var tb = (TextBox)e.OriginalSource;
                if (tb.SelectionStart != 0)
                {
                    EmployeesComboBox.SelectedItem = null;
                }
                if (tb.SelectionStart == 0 && EmployeesComboBox.SelectedItem == null)
                {
                    EmployeesComboBox.IsDropDownOpen = false;
                }

                EmployeesComboBox.IsDropDownOpen = true;
                if (EmployeesComboBox.SelectedItem == null)
                {
                    CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(EmployeesComboBox.ItemsSource);
                    cv.Filter = s =>
                    {
                        if (s is not Employee employee || employee is null) return false;
                        string fio = employee.Surname + " " + employee.Name + " " + employee.Patronymic;
                        return fio.Contains(EmployeesComboBox.Text, StringComparison.CurrentCultureIgnoreCase);
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        

    }
}
