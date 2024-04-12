using System;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers.Messages;
using MaterialDesignThemes.Wpf;
using NLog;
using NLog.Targets;

namespace FarmlandGuide.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IRecipient<ErrorMessage>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Logout");
            _isNonCloseButtonClicked = true;
            var authWindow = new AuthorizationWindow();
            authWindow.Show();
            Close();
        }

        private bool _isNonCloseButtonClicked;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (_isNonCloseButtonClicked)
                {
                    if (!e.Cancel) return;
                    Logger.Debug("Main window exit");
                    _isNonCloseButtonClicked = false;
                }
                else
                {
                    e.Cancel = true;
                    Logger.Debug("App shutdown from main window");
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Something went wrong");
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Отправьте мне этот файл:  {((FileTarget)LogManager.Configuration.AllTargets[1]).FileName} \n Текст ошибки: {ex.Message}"));
            }
        }
        public void Receive(ErrorMessage message)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                    {
                        HostDialogs.CurrentSession?.Close();
                        // Создание StackPanel
                        var stackPanel = new StackPanel
                        {
                            Margin = new Thickness(10),
                            MaxWidth = 300
                        };

                        // Создание вложенного StackPanel
                        var innerStackPanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(5)
                        };
                        innerStackPanel.SetValue(Grid.ColumnSpanProperty, 2);

                        // Добавление PackIcon во вложенный StackPanel
                        var packIcon = new PackIcon
                        {
                            Kind = PackIconKind.AlertCircle,
                            Margin = new Thickness(0, 0, 5, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        innerStackPanel.Children.Add(packIcon);

                        // Добавление TextBlock во вложенный StackPanel
                        var errorTextBlock = new TextBlock
                        {
                            Text = "Ошибка",
                            Style = (Style)Application.Current.FindResource("MaterialDesignSubtitle1TextBlock")
                        };
                        innerStackPanel.Children.Add(errorTextBlock);

                        // Добавление вложенного StackPanel в основной StackPanel
                        stackPanel.Children.Add(innerStackPanel);

                        // Добавление TextBlock
                        var messageTextBlock = new TextBlock
                        {
                            Style = (Style)Application.Current.FindResource("MaterialDesignBody2TextBlock"),
                            Text = message.Value,
                            Margin = new Thickness(5, 10, 5, 10),
                            TextWrapping = TextWrapping.Wrap
                        };
                        stackPanel.Children.Add(messageTextBlock);

                        // Создание Button
                        var button = new Button
                        {
                            Command = DialogHost.CloseDialogCommand,
                            Content = "Ясно",
                            IsCancel = true,
                            Style = (Style)Application.Current.FindResource("MaterialDesignRaisedButton")
                        };
                        button.SetValue(Grid.RowProperty, 5);
                        button.HorizontalAlignment = HorizontalAlignment.Center;
                        button.VerticalAlignment = VerticalAlignment.Bottom;
                        button.Margin = new Thickness(5, 10, 5, 10);


                        // Добавление Button в StackPanel
                        stackPanel.Children.Add(button);

                        DialogHost.Show(stackPanel);
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Can`t display info about error to user");
            }
        }

    }
}
