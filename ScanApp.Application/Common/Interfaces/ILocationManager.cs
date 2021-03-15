using System.Collections.Generic;
using System.Threading.Tasks;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.Entities;

namespace ScanApp.Application.Common.Interfaces
{
    public interface ILocationManager
    {
        Task<Result<List<Location>>> GetAllLocations();
        Task<Result<Location>> GetLocationByName(string name);
        Task<Result<Location>> GetLocationById(string index);
        Task<Result<Location>> AddNewLocation(Location location);
        Task<Result<Location>> AddNewLocation(string locationName);
        Task<Result> RemoveLocation(Location location);
    }
}