using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public static class TempModels
    {
        public static List<Enterprise> Enterprises = new()
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
        public static List<Employee> Employees = new()
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
        public static List<WorkSession> WorkSessions = new()
        {
            new WorkSession(DateTime.Parse("01/01/2024 08:00:00"), DateTime.Parse("01/01/2024 20:00:00"), "Работа"),
            new WorkSession(DateTime.Parse("01/02/2024 08:00:00"), DateTime.Parse("01/02/2024 20:00:00"), "Работа"),
            new WorkSession(DateTime.Parse("01/03/2024 08:00:00"), DateTime.Parse("01/03/2024 20:00:00"), "Работа"),
            new WorkSession(DateTime.Parse("01/04/2024 08:00:00"), DateTime.Parse("01/04/2024 20:00:00"), "Работа"),
            new WorkSession(DateTime.Parse("01/05/2024 08:00:00"), DateTime.Parse("01/05/2024 20:00:00"), "Отпуск"),
            new WorkSession(DateTime.Parse("02/04/2024 08:00:00"), DateTime.Parse("02/04/2024 20:00:00"), "Работа"),
        };
        public static List<Task> Tasks = new()
        {
            new Task(1, Employees[0].FIO, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-5), "Исполнено", "Полив полей"),
            new Task(2, Employees[1].FIO, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-1), "Провалено", "Уборка урожая"),
            new Task(3, Employees[2].FIO, DateTime.Now, DateTime.Now.AddDays(2), "Выполняется", "Посадка культур"),
            new Task(4, Employees[3].FIO, DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-3), "Исполнено", "Обработка почвы"),
            new Task(5, Employees[4].FIO, DateTime.Now.AddDays(-2), DateTime.Now.AddDays(3), "Выполняется", "Сбор урожая"),
            new Task(6, Employees[5].FIO, DateTime.Now.AddDays(1), DateTime.Now.AddDays(5), "Запланировано", "Подготовка теплиц"),
            new Task(7, Employees[6].FIO, DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-15), "Исполнено", "Ремонт оборудования"),
            new Task(8, Employees[7].FIO, DateTime.Now.AddDays(-15), DateTime.Now.AddDays(-10), "Провалено", "Покраска забора"),
            new Task(9, Employees[8].FIO, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(0), "Исполнено", "Установка системы капельного полива"),
            new Task(10, Employees[9].FIO, DateTime.Now.AddDays(2), DateTime.Now.AddDays(7), "Запланировано", "Инвентаризация семян"),
        };
        public static List<ProductionProcess> ProductionProcesses = new()
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

    }
}
