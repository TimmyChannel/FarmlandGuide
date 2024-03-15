using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ProcessesPage.xaml
    /// </summary>
    public partial class ProcessesPage : Page
    {
        public ProcessesPage()
        {
            InitializeComponent();
            var ProductionProcesses = new ObservableCollection<ProductionProcess>()
    {
        new ProductionProcess("Посев", "Посев сельскохозяйственных культур", 1000m),
        new ProductionProcess("Полив", "Автоматический и ручной полив полей", 500m),
        new ProductionProcess("Внесение удобрений", "Распределение минеральных и органических удобрений", 1500m),
        new ProductionProcess("Обработка почвы", "Пахота, культивация, боронование", 2000m),
        new ProductionProcess("Уборка урожая", "Сбор сельскохозяйственной продукции", 3000m),
        new ProductionProcess("Фитосанитарная обработка", "Защита растений от вредителей и болезней", 1200m),
        new ProductionProcess("Хранение продукции", "Организация условий для хранения урожая", 2500m),
        new ProductionProcess("Сортировка и фасовка", "Подготовка продукции к продаже или переработке", 800m),
        new ProductionProcess("Подготовка к следующему сезону", "Восстановление и подготовка почвы и инфраструктуры", 1000m),
        new ProductionProcess("Поддержание инфраструктуры", "Ремонт и обслуживание фермерских машин и оборудования", 2000m),
        new ProductionProcess("Животноводство", "Уход за сельскохозяйственными животными, кормление", 4000m),
        new ProductionProcess("Переработка продукции", "Производство продуктов питания из сырья", 5000m),
        new ProductionProcess("Логистика и доставка", "Организация транспортировки продукции", 3000m),
        new ProductionProcess("Мониторинг состояния культур", "Использование дронов и спутникового наблюдения для анализа", 2200m),
        new ProductionProcess("Участие в сельскохозяйственных выставках", "Представление продукции предприятия на выставках", 1500m),
        new ProductionProcess("Разработка новых технологий", "Инновации в сельском хозяйстве", 6000m),
        new ProductionProcess("Эко-фермерство", "Внедрение устойчивых и экологичных методов ведения хозяйства", 3500m),
        new ProductionProcess("Маркетинг и реклама", "Продвижение продукции и бренда на рынке", 2000m),
        new ProductionProcess("Управление отходами", "Компостирование и утилизация отходов", 1000m),
        new ProductionProcess("Обучение и развитие персонала", "Повышение квалификации сотрудников", 3000m)
    };
            ProcessGrid.ItemsSource = ProductionProcesses;
        }
    }
    public class ProductionProcess
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; } // Допустим, стоимость выполнения процесса

        public ProductionProcess(string name, string description, decimal cost)
        {
            Name = name;
            Description = description;
            Cost = cost;
        }
    }
}
