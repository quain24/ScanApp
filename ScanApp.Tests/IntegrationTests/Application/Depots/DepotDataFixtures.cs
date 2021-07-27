using ScanApp.Application.HesHub.Depots;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Tests.IntegrationTests.Application.Depots
{
    public static class DepotDataFixtures
    {
        public class DepotBuilder
        {
            private Depot _depot = new Depot(0, "depot name", "0123456", "wp@wp.pl",
                Address.Create("street", "00111", "city", "country"))
            {
                Id = 0
            };

            public DepotBuilder WithId(int id)
            {
                _depot.Id = id;
                return this;
            }

            public DepotBuilder WithName(string name)
            {
                _depot.ChangeName(name);
                return this;
            }

            public DepotBuilder WithPhoneNumber(string number)
            {
                _depot.ChangePhoneNumber(number);
                return this;
            }

            public DepotBuilder WithEmail(string mail)
            {
                _depot.ChangeEmail(mail);
                return this;
            }

            public DepotBuilder WithAddress(Address address)
            {
                _depot.ChangeAddress(address);
                return this;
            }

            public DepotBuilder WithGate(Gate gate)
            {
                _depot.DefaultGate = gate;
                return this;
            }

            public DepotBuilder WithTrailerType(TrailerType trailer)
            {
                _depot.DefaultTrailer = trailer;
                return this;
            }

            public Depot Build()
            {
                return _depot;
            }

            public DepotModel BuildAsModel(Depot depot = null)
            {
                var d = depot ?? _depot;
                return new DepotModel()
                {
                    DefaultGate = d.DefaultGate is null
                        ? null
                        : new GateModel()
                        {
                            Id = d.DefaultGate.Id,
                            Version = d.DefaultGate.Version,
                            Number = d.DefaultGate.Number
                        },
                    DefaultTrailer = d.DefaultTrailer is null
                        ? null
                        : new TrailerTypeModel()
                        {
                            Id = d.DefaultTrailer.Id,
                            Version = d.DefaultTrailer.Version,
                            Name = d.DefaultTrailer.Name
                        },
                    City = d.Address.City,
                    Id = d.Id,
                    Version = d.Version,
                    Name = d.Name,
                    Country = d.Address.Country,
                    DistanceToDepot = d.DistanceFromHub,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    StreetName = d.Address.StreetName,
                    ZipCode = d.Address.ZipCode
                };
            }
        }
    }
}