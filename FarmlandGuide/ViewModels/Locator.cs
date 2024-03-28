namespace FarmlandGuide.ViewModels
{
    public class Locator
    {

        public EmployeesPageViewModel EmployeesPageViewModel { get; } = new();
        public EnterprisesPageViewModel EnterprisesPageViewModel { get; } = new();
        public EnterprisesTasksPageViewModel EnterprisesTasksPageViewModel { get; } = new();
        public PersonalStatisticsPageViewModel PersonalStatisticsPageViewModel { get; } = new();
        public ProcessesPageViewModel ProcessesPageViewModel { get; } = new();
        public WorkSessionsViewModel WorkSessionsViewModel { get; } = new();
        public AuthorizationWindowViewModel AuthorizationWindowViewModel { get; } = new();
        public MainWindowViewModel MainWindowViewModel { get; } = new();
    }
}
