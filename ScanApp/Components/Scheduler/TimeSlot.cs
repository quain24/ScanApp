using System;

namespace ScanApp.Components.Scheduler
{
    public class TimeSlot
    {
        public int StartHour { get; private set; }
        public int EndHour { get; private set; }

        public override string ToString()
        {
            return StartHour.ToString() + ":00 - " + EndHour.ToString() +
                   ":00";
        }

        public TimeSlot(int startHour, int endHour)
        {
            if (endHour - startHour != 1)
            {
                throw new ArgumentException("Difference between start and end hour must be 1.");
            }

            StartHour = startHour;
            EndHour = endHour;
        }
    }
}