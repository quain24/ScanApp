using ScanApp.Models.SpareParts;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Data
{
    public class WeatherForecast
    {
        public WeatherForecast()
        {
        }

        public DateTime? Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public Version Version { get; set; } = Version.Create("teststamp");

        public double Number = 10.21;

        public SparePartGUIModel Model { get; set; } = new SparePartGUIModel("name", 13);
    }
}