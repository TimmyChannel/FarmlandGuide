using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using Employee = FarmlandGuide.Models.Entities.Employee;

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
