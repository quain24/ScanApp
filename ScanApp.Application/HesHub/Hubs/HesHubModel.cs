﻿using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.Hubs
{
    public class HesHubModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhonePrefix { get; set; }
        public string PhoneNumber { get; set; }
        public Version Version { get; set; }
    }
}