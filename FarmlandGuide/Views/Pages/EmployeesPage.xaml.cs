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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            List<Employee> list = new()
            {
                new Employee("Иванов Петр Сергеевич", "Фермер", "5/2 с 08:00 – 20:00"),
                new Employee("Смирнова Анна Ивановна", "Агроном", "5/2 с 08:00 – 20:00"),
                new Employee("Козлов Дмитрий Александрович", "Ветеринар", "5/2 с 08:00 – 20:00"),
                new Employee("Павлова Елена Владимировна", "Бухгалтер", "5/2 с 08:00 – 20:00"),
                new Employee("Морозова Ольга Алексеевна", "Лаборант", "5/2 с 08:00 – 20:00"),
                new Employee("Никитин Алексей Андреевич", "Машинист", "5/2 с 08:00 – 20:00"),
                new Employee("Федорова Мария Павловна", "Специалист по растениеводству", "5/2 с 08:00 – 20:00"),
                new Employee("Соколов Владимир Николаевич", "Специалист по животноводству", "5/2 с 08:00 – 20:00"),
                new Employee("Андреева Екатерина Дмитриевна", "Менеджер по закупкам", "5/2 с 08:00 – 20:00"),
                new Employee("Белов Артем Степанович", "Агротехник", "5/2 с 08:00 – 20:00"),
            };
            List<Session> sessions = new()
        {
            new Session(DateTime.Parse("01/01/2024 08:00:00"), DateTime.Parse("01/01/2024 20:00:00"), "Работа"),
            new Session(DateTime.Parse("01/02/2024 08:00:00"), DateTime.Parse("01/02/2024 20:00:00"), "Работа"),
            new Session(DateTime.Parse("01/03/2024 08:00:00"), DateTime.Parse("01/03/2024 20:00:00"), "Работа"),
            new Session(DateTime.Parse("01/04/2024 08:00:00"), DateTime.Parse("01/04/2024 20:00:00"), "Работа"),
            new Session(DateTime.Parse("01/05/2024 08:00:00"), DateTime.Parse("01/05/2024 20:00:00"), "Отпуск"),
            new Session(DateTime.Parse("02/04/2024 08:00:00"), DateTime.Parse("02/04/2024 20:00:00"), "Работа"),
        };
            SessionGrid.ItemsSource = sessions;
            EmployeesGrid.ItemsSource = list;
        }
    }
    record Employee(string FIO, string Position, string WorkSchedule);
    record Session(DateTime StartTime, DateTime EndTime, string ActionType);
}
