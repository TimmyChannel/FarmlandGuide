using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EnterprisesPage.xaml
    /// </summary>
    public partial class EnterprisesPage : Page
    {
        public EnterprisesPage()
        {
            InitializeComponent();
            EnterpiseGrid.ItemsSource = TempModels.Enterprises;
        }
    }
}
