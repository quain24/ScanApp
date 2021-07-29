using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.TelerikScheduler
{
    public class SchedulerResourceService
    {
        public async Task<List<Resource>> GetDepotsAsync()
        {
            return await Task.FromResult(GetDepots());
        }

        public List<Resource> GetDepots()
        {
            List<Resource> result = new List<Resource>();

            result.Add(new Resource()
            {
                Text = "Depot 1",
                Value = "1",
                Color = "#6eb3fa"
            });
            result.Add(new Resource()
            {
                Text = "Depot 2",
                Value = "2",
                Color = "#f58a8a"
            });
            result.Add(new Resource()
            {
                Text = "Depot 3",
                Value = "3",
                Color = "#f58a8a"
            });
            result.Add(new Resource()
            {
                Text = "Depot 4",
                Value = "5",
                Color = "#f58a8a"
            });

            return result;
        }
    }
}
