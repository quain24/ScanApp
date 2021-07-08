using AutoFixture;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class DepotFixtures
    {
        public class DepotBuilder
        {
            private int _id;
            private string _name;
            private string _phoneNumber;
            private string _email;
            private Address _address;
            public Fixture Fixture { get; } = new Fixture();
            private Random Rand { get; } = new(DateTime.Now.Millisecond);

            public DepotBuilder()
            {
                Fixture.Register<Depot>(() =>
                new Depot(Fixture.Create<int>(),
                "name" + DateTime.Now.TimeOfDay,
                Rand.Next(100000, 9999999).ToString(),
                    $"email{Rand.Next(0, 999)}@wp.pl",
                Address.Create(Rand.Next(1, 900000).ToString(),
                Rand.Next(10000, 99999).ToString(),
                    $"{Rand.Next(0, 999)} city",
                    $"{Rand.Next(0, 999)} country")));
            }

            public DepotBuilder Create()
            {
                return this;
            }

            public DepotBuilder CreateWithRandomValidData()
            {
                var tmp = Fixture.Create<Depot>();
                _id = tmp.Id;
                _name = tmp.Name;
                _phoneNumber = tmp.PhoneNumber;
                _email = tmp.Email;
                _address = tmp.Address;

                return this;
            }

            public DepotBuilder WithId(int id)
            {
                _id = id;
                return this;
            }

            public DepotBuilder WithName(string name)
            {
                _name = name;
                return this;
            }

            public DepotBuilder WithPhoneNumber(string number)
            {
                _phoneNumber = number;
                return this;
            }

            public DepotBuilder WithEmail(string mail)
            {
                _email = mail;
                return this;
            }

            public DepotBuilder WithAddress(Address address)
            {
                _address = address;
                return this;
            }

            public DepotBuilder WithDefaultValidAddress()
            {
                _address = Address.Create("street name", "12-345", "City name", "Country name");
                return this;
            }

            public Depot Build()
            {
                return new Depot(_id, _name, _phoneNumber, _email, _address);
            }
        }
    }
}