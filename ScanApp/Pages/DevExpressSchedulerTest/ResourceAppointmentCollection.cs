using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.DevExpressSchedulerTest
{
    public static partial class ResourceAppointmentCollection {
        public static List<ResourceAppointment> GetAppointments() {
            DateTime date = DateTime.Now.Date;
            var dataSource = new List<ResourceAppointment>() {
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Install New Router in Dev Room",
                //    StartDate = date + (new TimeSpan(0, 10, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                //    Status = 1,
                //    DepotId = 0
                //},
                //new ResourceAppointment {
                //    Caption = "Upgrade Personal Computers",
                //    Accepted = false,
                //    StartDate = date + (new TimeSpan(0,  13, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 14, 30, 0)),
                //    Status = 1,
                //    DepotId = 0
                //},
                //new ResourceAppointment {
                //    Caption = "Website Re-Design Plan",
                //    Accepted = false,
                //    StartDate = date + (new TimeSpan(1, 9, 30, 0)),
                //    EndDate = date + (new TimeSpan(1, 11, 30, 0)),
                //    Status = 1,
                //    DepotId = 0
                //},
                //new ResourceAppointment {
                //    Caption = "New Brochures",
                //    Accepted = true,
                //    StartDate = date + (new TimeSpan(1, 13, 30, 0)),
                //    EndDate = date + (new TimeSpan(1, 15, 15, 0)),
                //    Status = 1,
                //    DepotId = 0
                //},
                //new ResourceAppointment {
                //    Caption = "Book Flights to San Fran for Sales Trip",
                //    Accepted = false,
                //    StartDate = date + (new TimeSpan(1, 12, 0, 0)),
                //    EndDate = date + (new TimeSpan(1, 13, 0, 0)),
                //    AllDay = true,
                //    Status = 1,
                //    DepotId = 0
                //},
                //new ResourceAppointment {
                //    Caption = "Approve Personal Computer Upgrade Plan",
                //    Accepted = true,
                //    StartDate = date + (new TimeSpan(0, 10, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                //    DepotId = 5,
                //    Status = 1
                //},
                //new ResourceAppointment {
                //    Caption = "Final Budget Review",
                //    Accepted = true,
                //    StartDate = date + (new TimeSpan(0, 13, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 15, 0, 0)),
                //    Status = 1,
                //    DepotId = 1
                //},
                //new ResourceAppointment {
                //    Caption = "Install New Database",
                //    Accepted = false,
                //    StartDate = date + (new TimeSpan(0, 9, 45, 0)),
                //    EndDate = date + (new TimeSpan(1, 11, 15, 0)),
                //    Status = 1,
                //    DepotId = 1
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Approve New Online Marketing Strategy",
                //    StartDate = date + (new TimeSpan(1,  12, 0, 0)),
                //    EndDate = date + (new TimeSpan(1, 14, 0, 0)),
                //    Status = 1,
                //    DepotId = 1
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Customer Workshop",
                //    StartDate = date + (new TimeSpan(0,  11, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 12, 0, 0)),
                //    AllDay = true,
                //    Status = 1,
                //    DepotId = 2
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Prepare 2021 Marketing Plan",
                //    StartDate = date + (new TimeSpan(0,  11, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 13, 30, 0)),
                //    Status = 1,
                //    DepotId = 2
                //},
                //new ResourceAppointment {
                //    Accepted = false,
                //    Caption = "Brochure Design Review",
                //    StartDate = date + (new TimeSpan(0, 14, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 15, 30, 0)),
                //    Status = 1,
                //    DepotId = 2
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Create Icons for Website",
                //    StartDate = date + (new TimeSpan(1, 10, 0, 0)),
                //    EndDate = date + (new TimeSpan(1, 11, 30, 0)),
                //    Status = 1,
                //    DepotId = 1
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Launch New Website",
                //    StartDate = date + (new TimeSpan(1, 12, 20, 0)),
                //    EndDate = date + (new TimeSpan(1, 14, 0, 0)),
                //    Status = 1,
                //    DepotId = 2
                //},
                //new ResourceAppointment {
                //    Accepted = false,
                //    Caption = "Upgrade Server Hardware",
                //    StartDate = date + (new TimeSpan(1, 9, 0, 0)),
                //    EndDate = date + (new TimeSpan(1, 12, 0, 0)),
                //    Status = 1,
                //    DepotId = 2
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Book Flights to San Fran for Sales Trip",
                //    StartDate = date + (new TimeSpan(0, 14, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 17, 0, 0)),
                //    Status = 1,
                //    DepotId = 3
                //},
                //new ResourceAppointment {
                //    Accepted = true,
                //    Caption = "Approve New Online Marketing Strategy",
                //    StartDate = date + (new TimeSpan(0,  12, 0, 0)),
                //    EndDate = date + (new TimeSpan(0, 15, 0, 0)),
                //    Status = 1,
                //    DepotId = 4
                //}
            };
            return dataSource;
        }

        public static List<Resource> GetResourcesForGrouping() {
            return GetResources().ToList();
        }

        public static List<Resource> GetResources() {
            return new List<Resource>() {
                new Resource() { Id=0 , Name="Depot 1", GroupId=100, BackgroundCss="dx-green-color", TextCss="text-white" },
                new Resource() { Id=1 , Name="Depot 2", GroupId=101, BackgroundCss="dx-orange-color", TextCss="text-white" },
                new Resource() { Id=2 , Name="Depot 3", GroupId=100, BackgroundCss="dx-red-color", TextCss="text-white" },
                new Resource() { Id=3 , Name="Depot 4", GroupId=100, BackgroundCss="dx-pink-color", TextCss="text-white" },
                new Resource() { Id=4 , Name="Depot 5", GroupId=100, BackgroundCss="dx-blue-color", TextCss="text-white" },
                new Resource() { Id=5 , Name="Depot 6", GroupId=100, BackgroundCss="dx-green-color", TextCss="text-white" },
                new Resource() { Id=6 , Name="Depot 7", GroupId=100, BackgroundCss="dx-red-color", TextCss="text-white" },
                new Resource() { Id=7 , Name="Depot 8", GroupId=100, BackgroundCss="dx-yellow-color", TextCss="text-white" },
                new Resource() { Id=8 , Name="Depot 9", GroupId=100, BackgroundCss="dx-blue-color", TextCss="text-white" },
                new Resource() { Id=9 , Name="Depot 10", GroupId=100, BackgroundCss="dx-purple-color", TextCss="text-white" }
            };
        }
    }
}
