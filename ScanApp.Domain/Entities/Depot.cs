using ScanApp.Domain.ValueObjects;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    public class Depot
    {
        /// <summary>
        /// Database Id of this entity, primary key.
        /// </summary>
        public int Id { get; set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string PhoneNumber { get; private set; }
        public double DistanceFromHub { get; private set; }
        public Trailer DefaultTrailer { get; set; }
        public Gate DefaultGate { get; set; }
        public Version Version { get; private set; } = Version.Empty();

        /// <summary>
        /// For compliance with EF Core.
        /// </summary>
        private Depot()
        {
        }

        public Depot(int id, string name, string phoneNumber, string email, Address address)
        {
            Id = id;
            ChangeName(name);
            ChangeAddress(address);
            ChangePhoneNumber(phoneNumber);
            ChangeEmail(email);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Depot must have name.", nameof(name));
            Name = name;
        }

        public void ChangeAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address), $"Provided {nameof(Address)} object was null.");
        }

        public void ChangePhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Phone number must be provided", nameof(number));
            PhoneNumber = number;
        }

        public void ChangeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email address must be provided", nameof(email));
            if (email.Contains('@') is false && email.Contains('.') is false)
                throw new ArgumentException($"Email address ({email}) is not a proper email.", nameof(email));
            Email = email;
        }

        public void ChangeDistanceToHub(double distanceInKm)
        {
            DistanceFromHub = distanceInKm >= 0
                ? distanceInKm
                : throw new ArgumentException("Distance must be 0 or more", nameof(distanceInKm));
        }

        public void ChangeVersion(Version version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}