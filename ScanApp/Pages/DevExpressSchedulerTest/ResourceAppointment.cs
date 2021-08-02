using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.DevExpressSchedulerTest
{
    public static partial class ResourceAppointmentCollection {
        public class ResourceAppointment {
            public ResourceAppointment() { }
            public bool Accepted { get; set; }
            public int AppointmentType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Caption { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public int? Label { get; set; }
            public int Status { get; set; }
            public bool AllDay { get; set; }
            public string Recurrence { get; set; }
            public int? DepotId { get; set; }
        }
    }
}
