using ScanApp.Components.Scheduler;
using System;

namespace ScanApp.Models.Scheduler
{
    public class HesAppointmentModel
    {
        public int DepotID { get; set; }
        public string Spedition { get; set; }
        public string Company { get; set; }
        public string IdentificationNumber { get; set; }
        public string Note { get; set; }
        public DateTime? Date { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public bool Loading { get; set; }
        public bool Unloading { get; set; }
        public int GridLocation { get; set; }

        public HesAppointmentModel(DateTime date, int hourFrom, int hourTo)
        {
            if (date == new DateTime())
                throw new ArgumentException("Value must be specified", nameof(Date));

            Date = date;

            TimeSlot = new TimeSlot(hourFrom, hourTo);
        }

    }
}