using ScanApp.Domain.ValueObjects;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Domain.Entities
{
    public class HesDepot
    {
        public int Id { get; set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string PhonePrefix { get; private set; }
        public string PhoneNumber { get; private set; }
        public Version Version { get; private set; }

        /// <summary>
        /// For compliance with EF Core.
        /// </summary>
        private HesDepot()
        {
        }

        public HesDepot(int id, string name, Address address, string phonePrefix, string phoneNumber, string email)
        {
            Id = id;
            ChangeName(name);
            ChangeAddress(address);
            ChangePhonePrefix(phonePrefix);
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

        public void ChangePhonePrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentException("There must be a phone number prefix given.", nameof(prefix));
            PhonePrefix = prefix;
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
            //Version = Version.Create(Guid.NewGuid().ToString());
            Email = email;
        }

        public void ChangeVersion(Version version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}