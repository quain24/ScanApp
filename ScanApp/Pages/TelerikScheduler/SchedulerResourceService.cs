using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.TelerikScheduler
{
    public class SchedulerResourceService
    {
        public async Task<List<Resource>> GetRoomsAsync()
        {
            return await Task.FromResult(GetRooms());
        }

        public List<Resource> GetRooms()
        {
            List<Resource> result = new List<Resource>();

            result.Add(new Resource()
            {
                Text = "Small meeting room",
                Value = "1",
                Color = "#6eb3fa"
            });
            result.Add(new Resource()
            {
                Text = "Big meeting room",
                Value = "2",
                Color = "#f58a8a"
            });

            return result;
        }

        public async Task<List<Resource>> GetManagersAsync()
        {
            return await Task.FromResult(GetManagers());
        }

        public List<Resource> GetManagers()
        {
            List<Resource> result = new List<Resource>();

            result.Add(new Resource()
            {
                Text = "Alex",
                Value = "1",
                Color = "#f8a398"
            });
            result.Add(new Resource()
            {
                Text = "Bob",
                Value = "2",
                Color = "#51a0ed"
            });
            result.Add(new Resource()
            {
                Text = "Charlie",
                Value = "3",
                Color = "#56ca85"
            });

            return result;
        }
    }
}
