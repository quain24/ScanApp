using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScanApp.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing basic address (Country, City, Street, Street number, Zip code).
    /// </summary>
    public sealed class Address : ValueObject
    {
        public string StreetName { get; }
        public string StreetNumber { get; }
        public string ZipCode { get; }
        public string City { get; }
        public string Country { get; }

        /// <summary>
        /// Creates new instance of <see cref="Address"/> value object.
        /// </summary>
        /// <param name="streetName">Name of the street.</param>
        /// <param name="streetNumber">Street address number (can be <see langword="null"/> if not present).</param>
        /// <param name="zipCode">Zip code</param>
        /// <param name="city">Name of the city.</param>
        /// <param name="country">Country where given address is located.</param>
        /// <returns>A new instance of <see cref="Address"/>.</returns>
        /// <exception cref="ArgumentException">One of parameters is null, empty or whitespace, excluding street number - it can be null, but not empty or whitespace.</exception>
        public static Address Create(string streetName, string streetNumber, string zipCode, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                throw new ArgumentException("Street name cannot be null or whitespace.", nameof(streetName));
            if (streetNumber is not null && string.IsNullOrWhiteSpace(streetNumber))
                throw new ArgumentException("Street number cannot be whitespace - it can either be null or must contain actual data.", nameof(streetNumber));
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("Zip code cannot be null or whitespace.", nameof(zipCode));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City name cannot be null or whitespace.", nameof(city));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country name cannot be null or whitespace.", nameof(country));

            return new Address(streetName, streetNumber, zipCode, city, country);
        }

        private Address(string streetName, string streetNumber, string zipCode, string city, string country)
        {
            StreetName = streetName;
            StreetNumber = streetNumber;
            ZipCode = zipCode;
            City = city;
            Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StreetName;
            yield return StreetNumber;
            yield return ZipCode;
            yield return City;
            yield return Country;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(StreetName).Append(' ');
            if (StreetNumber is not null) builder.Append(StreetNumber);
            builder.Append(", ");
            builder.Append(ZipCode).Append(' ').Append(City).Append(", ").Append(Country);
            return builder.ToString();
        }
    }
}