using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
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
using System.Windows.Shapes;

namespace FarmlandGuide.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window, IRecipient<LoggedUserMessage>
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(LoggedUserMessage message)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
 