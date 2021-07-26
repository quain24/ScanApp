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
        public DateTime? Date { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public HesAppointmentModel(DateTime date, int hourFrom, int hourTo, int depotID)
        {
            if (date == new DateTime())
                throw new ArgumentException("Value must be specified", nameof(Date));
            if (depotID <= 0)
                throw new ArgumentException("DepotID cannot be equal or smaller than 0.", nameof(depotID));
            DepotID = depotID;
            Date = date;

            TimeSlot = new TimeSlot(hourFrom, hourTo);
        }

    }
}