using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TmsSystem.ViewModels
{
    public class CreateTourViewModel
    {

        public int TourId { get; set; }  // <-- להוסיף

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        // לוח זמנים
        public List<ScheduleItemViewModel> Schedule { get; set; } = new List<ScheduleItemViewModel>();

        // כולל / לא כולל
        public List<string> Includes { get; set; } = new();
        public List<string> Excludes { get; set; } = new();
    }

    public class ScheduleItemViewModel
    {
        [Required]
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
