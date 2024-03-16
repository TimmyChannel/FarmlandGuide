using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.Models
{
    public record WorkSession(DateTime StartTime, DateTime EndTime, string ActionType);
}
