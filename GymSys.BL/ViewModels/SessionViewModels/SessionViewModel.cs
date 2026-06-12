using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.ViewModels.SessionViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public int Capacity { get; set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrainerName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public int AvailableSlots { get; set; }

        //Computed
        public string DateDisplay => $"{StartDate:MM dd, yyyy}";
        public string TimeRangeDisplay => $"{EndDate:hh:mm tt} - {StartDate:hh:mm tt}";
        public TimeSpan Duration => EndDate - StartDate;
        public string Status
        {
            get
            {
                if (StartDate > DateTime.Now)
                    return "Upcoming";
                else if (StartDate <= DateTime.Now && EndDate >= DateTime.Now)
                    return "Ongoing";
                else
                    return "Completed";
            }
        }

    }
}
