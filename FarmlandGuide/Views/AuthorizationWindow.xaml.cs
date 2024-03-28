using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using System.Windows;

namespace FarmlandGuide.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window, IRecipient<LoggedUserMessage>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public AuthorizationWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(LoggedUserMessage message)
        {
            Logger.Debug("Closing...");
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

    }
}
 