using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;

namespace ScanApp.Domain.Entities
{
    /// <summary>
    /// Represents a single Hes depot / transport hub.
    /// </summary>
    public class Depot : VersionedEntity
    {
        /// <summary>
        /// Database Id of this entity, primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets a contact email for this depot.
        /// </summary>
        /// <value>Email address in <see cref="string"/>.</value>
        public string Email { get; private set; }

        /// <summary>
        /// Gets a name of this depot.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a physical location of this depot.
        /// </summary>
        /// <see cref="Address"/> object that contains all address data.
        public Address Address { get; private set; }

        /// <summary>
        /// Gets a contact phone number for this depot.
        /// </summary>
        /// <value>Phone number including prefix, as <see cref="string"/>.</value>
        public string PhoneNumber { get; private set; }

        /// <summary>
        /// Gets approximate distance to this depot from HSF HES warehouse.
        /// </summary>
        /// Distance in kilometers as <see cref="double"/>
        public double DistanceFromHub { get; private set; }

        /// <summary>
        /// Gets default <see cref="TrailerType"/> type assigned to this depot.
        /// </summary>
        /// <value><see cref="TrailerType"/> entity representing a truck trailer type.</value>
        public TrailerType DefaultTrailer { get; set; }

        /// <summary>
        /// Gets default <see cref="Gate"/> assigned to this depot.
        /// </summary>
        /// <value><see cref="Gate"/> entity representing default gate in the HSF HES warehouse assigned to this depot.</value>
        public Gate DefaultGate { get; set; }

        /// <summary>
        /// For compliance with EF Core.
        /// </summary>
        private Depot()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="Depot"/> entity.
        /// </summary>
        /// <param name="id">ID used in the database.</param>
        /// <param name="name">Name of the depot.</param>
        /// <param name="phoneNumber">Contact number for the depot.</param>
        /// <param name="email">Contact email address for the depot.</param>
        /// <param name="address">Depot address.</param>
        public Depot(int id, string name, string phoneNumber, string email, Address address)
        {
            Id = id;
            ChangeName(name);
            ChangeAddress(address);
            ChangePhoneNumber(phoneNumber);
            ChangeEmail(email);
        }

        /// <summary>
        /// Changes depot name to a new given <see cref="name"/>.
        /// </summary>
        /// <param name="name">New name for this depot.</param>
        /// <exception cref="ArgumentException">New <paramref name="name"/> was <see langword="null"/>, empty or contained only whitespaces.</exception>
        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Depot must have name.", nameof(name));
            Name = name;
        }

        /// <summary>
        /// Changed depot address to a new given <paramref name="address"/>.
        /// </summary>
        /// <param name="address">New address for this depot.</param>
        /// <exception cref="ArgumentNullException">New <paramref name="address"/> was <see langword="null"/>.</exception>
        public void ChangeAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address), $"Provided {nameof(Address)} object was null.");
        }

        /// <summary>
        /// Changes depot phone number to a new given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">New phone number (including prefix) for this depot.</param>
        /// <exception cref="ArgumentException">New <paramref name="number"/> was <see langword="null"/>, empty or contained only whitespaces.</exception>
        public void ChangePhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Phone number must be provided", nameof(number));
            PhoneNumber = number;
        }

        /// <summary>
        /// Changes depot contact email address to a new given <paramref name="email"/>.
        /// </summary>
        /// <param name="email">New contact email for this depot.</param>
        /// <exception cref="ArgumentException">
        /// New <paramref name="email"/> was <see langword="null"/>, empty, contained only whitespaces or
        /// did not adhere to basic email naming - must contain single '@' symbol and at least one '.'.
        /// </exception>
        public void ChangeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email address must be provided", nameof(email));
            if (email.Count(x => x.Equals('@')) != 1 || email.Contains('.') is false || email.Contains(' '))
                throw new ArgumentException($"Email address ({email}) is not a proper email.", nameof(email));
            Email = email;
        }

        /// <summary>
        /// Changes distance to this depot from HSF HES warehouse to a new given <paramref name="distanceInKm"/>.
        /// </summary>
        /// <param name="distanceInKm">New distance in kilometers.</param>
        /// <exception cref="ArgumentException">New <paramref name="distanceInKm"/> was less than 0.</exception>
        public void ChangeDistanceToHub(double distanceInKm)
        {
            DistanceFromHub = distanceInKm >= 0
                ? distanceInKm
                : throw new ArgumentException("Distance must be 0 or more", nameof(distanceInKm));
        }
    }
}