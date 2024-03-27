using FarmlandGuide.Views;
using NLog;
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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _logger.Info("Logout");
            IsNonCloseButtonClicked = true;
            var authWindow = new AuthorizationWindow();
            authWindow.Show();
            this.Close();
        }
        bool IsNonCloseButtonClicked;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (IsNonCloseButtonClicked)
                {
                    if (e.Cancel)
                    {
                        _logger.Debug("Main window exit");
                        IsNonCloseButtonClicked = false;
                        return;
                    }
                }
                else
                {
                    e.Cancel = true;
                    _logger.Debug("App shutdown from main window");
                    App.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Something went wrong");
            }
        }
    }
}
