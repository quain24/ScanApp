using ScanApp.Domain.Exceptions;
using System.Linq;

namespace ScanApp.Domain.Entities
{
    public class Location
    {
        public Location(string name)
        {
            Validate(name);
            Name = name;
            NormalizedName = NormalizeName(name);
        }

        public Location(int id, string name)
        {
            Validate(name);
            Id = id;
            Name = name;
            NormalizedName = NormalizeName(name);
        }

        public int Id { get; set; }
        public string Name { get; private set; }

        public string NormalizedName { get; private set; }

        public void ChangeName(string name)
        {
            Validate(name);
            Name = name;
            NormalizedName = NormalizeName(Name);
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new LocationNameFormatException(name, "Location name cannot be null or empty / contain only whitespaces");

            if (name[0] == ' ' || name.Last() == ' ')
                throw new LocationNameFormatException(name, "Name cannot begin nor end with whitespace");

            if (name.Contains("  "))
                throw new LocationNameFormatException(name, "Multiple whitespaces detected one after another");
        }

        private static string NormalizeName(string name)
        {
            return name.ToUpperInvariant().Replace(' ', '_');
        }
    }
}