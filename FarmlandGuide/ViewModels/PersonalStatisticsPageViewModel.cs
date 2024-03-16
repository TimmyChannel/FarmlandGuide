using CommunityToolkit.Mvvm.ComponentModel;
using FarmlandGuide.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = FarmlandGuide.Models.Task;

namespace FarmlandGuide.ViewModels
{
    public partial class PersonalStatisticsPageViewModel : ObservableObject
    {
        public List<Task> Tasks
        {
            get => TempModels.Tasks.Where(x => x.Status != "Исполнено").ToList();
        }
        public List<WorkSession> WorkSessions => TempModels.WorkSessions;

    }
}
