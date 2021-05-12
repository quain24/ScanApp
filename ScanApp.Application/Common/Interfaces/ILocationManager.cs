using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides a way to manage location data in application.
    /// </summary>
    public interface ILocationManager
    {
        /// <summary>
        /// Retrieves all <see cref="Location"/> from app's data source.
        /// </summary>
        /// <returns>Result containing list of all locations.</returns>
        Task<Result<List<Location>>> GetAllLocations();

        /// <summary>
        /// Retrieves a <see cref="Location"/> with matching <paramref name="name"/> from app's data source.
        /// </summary>
        /// <returns>Result containing location with name matching given <paramref name="name"/>.</returns>
        Task<Result<Location>> GetLocationByName(string name);

        /// <summary>
        /// Retrieves a <see cref="Location"/> with matching <paramref name="index"/> from app's data source.
        /// </summary>
        /// <returns>Result containing location with Id matching given <paramref name="index"/>.</returns>
        Task<Result<Location>> GetLocationById(string index);

        /// <summary>
        /// Adds new <see cref="Location"/> to the app's data source.
        /// </summary>
        /// <param name="location">New location to be added.</param>
        /// <returns>Result containing newly created <paramref name="location"/>.</returns>
        Task<Result<Location>> AddNewLocation(Location location);

        /// <summary>
        /// Adds new location with given <paramref name="locationName"/> to the app's data source.
        /// </summary>
        /// <param name="locationName">Name of the new location to be added.</param>
        /// <returns>Result containing newly created <see cref="Location"/>.</returns>
        Task<Result<Location>> AddNewLocation(string locationName);

        /// <summary>
        /// Removes given <paramref name="location"/> from the app's data source.
        /// </summary>
        /// <param name="location">Location to be removed.</param>
        Task<Result> RemoveLocation(Location location);
    }
}