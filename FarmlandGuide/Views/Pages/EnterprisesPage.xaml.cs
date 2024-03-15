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
            List<Enterprise> enterprises = new()
        {
            new Enterprise("Зеленая поляна", "Республика Татарстан, Агрызский район, деревня Новая"),
            new Enterprise("Солнечное хутор", "Краснодарский край, Туапсинский район, поселок Солнечный"),
            new Enterprise("Родниковый край", "Ставропольский край, Лермонтовский район, село Родниковое"),
            new Enterprise("Северная долина", "Мурманская область, Мончегорский район, деревня Северная"),
            new Enterprise("Золотая нива", "Белгородская область, Шебекинский район, поселок Золотой"),
            new Enterprise("Луговая радуга", "Тульская область, Суворовский район, деревня Луговая"),
            new Enterprise("Родниковый исток", "Кировская область, Советский район, поселок Родниковый"),
            new Enterprise("Зеленый берег", "Краснодарский край, Геленджикский район, деревня Зеленая"),
            new Enterprise("Поляна снов", "Республика Башкортостан, Мелеузовский район, село Поляна"),
            new Enterprise("Лесная гавань", "Ленинградская область, Всеволожский район, поселок Лесной"),
        };
            EnterpiseGrid.ItemsSource = enterprises;
        }
    }
    public record Enterprise(string Name, string Address);
}
