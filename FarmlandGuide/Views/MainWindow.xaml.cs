using FarmlandGuide.Views;
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

namespace FarmlandGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsNonCloseButtonClicked = true;
            var authWindow = new AuthorizationWindow();
            authWindow.Show();
            this.Close();
        }
        bool IsNonCloseButtonClicked;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsNonCloseButtonClicked)
            {
                if (e.Cancel)
                {
                    IsNonCloseButtonClicked = false; 
                    return;
                }
            }
            else
            {
                e.Cancel = true;
                App.Current.Shutdown();
            }
        }
    }
}
