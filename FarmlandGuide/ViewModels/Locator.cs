using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public class Locator
    {
        public EmployeesPageViewModel EmployeesPageViewModel { get; }
        public EnterpisesPageViewModel EnterpisesPageViewModel { get; }
        public EnterprisesTasksPageViewModel EnterprisesTasksPageViewModel { get; }
        public PersonalStatisticsPageViewModel PersonalStatisticsPageViewModel { get; }
        public ProcessesPageViewModel ProcessesPageViewModel { get; }
        public Locator()
        {
            EmployeesPageViewModel = new EmployeesPageViewModel();
            EnterprisesTasksPageViewModel = new EnterprisesTasksPageViewModel();
            EnterpisesPageViewModel = new EnterpisesPageViewModel();
            PersonalStatisticsPageViewModel = new PersonalStatisticsPageViewModel();
            ProcessesPageViewModel = new ProcessesPageViewModel();
        }
    }
}
