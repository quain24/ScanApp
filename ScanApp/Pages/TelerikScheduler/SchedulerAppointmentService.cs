using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.TelerikScheduler
{
    public class SchedulerAppointmentService
    {
        public async Task<List<SchedulerAppointment>> GetAppointmentsAsync()
        {
            return await GetDummyAppointments();
        }

        private async Task<List<SchedulerAppointment>> GetDummyAppointments()
        {
            List<SchedulerAppointment> data = new List<SchedulerAppointment>();
            DateTime baselineTime = GetStartTime();

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddHours(2),
                End = baselineTime.AddHours(2).AddMinutes(30),
                Depot = "1"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                IsAllDay = true,
                Start = baselineTime.AddDays(-10),
                End = baselineTime.AddDays(-2),
                Depot = "2"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(5).AddHours(10),
                End = baselineTime.AddDays(5).AddHours(18),
                Depot = "1"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Start = baselineTime.AddHours(3).AddMinutes(30),
                End = baselineTime.AddHours(3).AddMinutes(45),
                Depot = "1",
                RecurrenceRule = "FREQ=MONTHLY;BYDAY=MO;BYSETPOS=2"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(3).AddHours(3),
                End = baselineTime.AddDays(3).AddHours(3).AddMinutes(45),
                Depot = "1"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(3).AddHours(1).AddMinutes(30),
                End = baselineTime.AddDays(3).AddHours(2).AddMinutes(30),
                Depot = "2"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(6),
                End = baselineTime.AddDays(11),
                IsAllDay = true,
                Depot = "1"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(3).AddHours(8).AddMinutes(30),
                End = baselineTime.AddDays(3).AddHours(11).AddMinutes(30),
                Depot = "2"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddHours(2).AddMinutes(15),
                End = baselineTime.AddHours(2).AddMinutes(30),
                Depot = "2"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something that comes up once in a while",
                Start = baselineTime.AddDays(-10).AddHours(1),
                End = baselineTime.AddDays(-10).AddHours(1).AddMinutes(30),
                Depot = "2",
                RecurrenceRule = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(3).AddHours(8).AddMinutes(30),
                End = baselineTime.AddDays(3).AddHours(11).AddMinutes(30),
                Depot = "3"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddHours(2).AddMinutes(15),
                End = baselineTime.AddHours(2).AddMinutes(30),
                Depot = "3"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something that comes up once in a while",
                Start = baselineTime.AddDays(-10).AddHours(1),
                End = baselineTime.AddDays(-10).AddHours(1).AddMinutes(30),
                Depot = "3",
                RecurrenceRule = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddDays(3).AddHours(8).AddMinutes(30),
                End = baselineTime.AddDays(3).AddHours(11).AddMinutes(30),
                Depot = "4"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something really detailed and important.",
                Start = baselineTime.AddHours(2).AddMinutes(15),
                End = baselineTime.AddHours(2).AddMinutes(30),
                Depot = "4"
            });

            data.Add(new SchedulerAppointment
            {
                Title = "LKW",
                Description = "Something that comes up once in a while",
                Start = baselineTime.AddDays(-10).AddHours(1),
                End = baselineTime.AddDays(-10).AddHours(1).AddMinutes(30),
                Depot = "4",
                RecurrenceRule = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR"
            });

            return await Task.FromResult(data);
        }

        public DateTime GetStartTime()
        {
            DateTime now = DateTime.Now;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime lastMonday = now.AddDays(-1 * diff);

            // return 8 AM on today's date for better visualization of the demos
            return new DateTime(lastMonday.Year, lastMonday.Month, lastMonday.Day, 8, 0, 0);
        }
    }
}
