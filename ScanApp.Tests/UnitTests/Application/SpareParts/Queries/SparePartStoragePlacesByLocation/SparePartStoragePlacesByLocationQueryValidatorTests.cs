using FluentAssertions;
using FluentValidation;
using ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    public class SparePartStoragePlacesByLocationQueryValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new SparePartStoragePlacesByLocationQueryValidator();

            subject.Should().BeOfType<SparePartStoragePlacesByLocationQueryValidator>()
                .And.BeAssignableTo(typeof(AbstractValidator<>));
        }

        [Fact]
        public void Validates_proper_query()
        {
            var query = new SparePartStoragePlacesByLocationQuery("location_id");
            var subject = new SparePartStoragePlacesByLocationQueryValidator();

            var result = subject.Validate(query);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Wont_validate_query_if_its_null()
        {
            var subject = new SparePartStoragePlacesByLocationQueryValidator();

            var result = subject.Validate(null as SparePartStoragePlacesByLocationQuery);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Wont_validate_invalid_location_id(string invalidLocationId)
        {
            var query = new SparePartStoragePlacesByLocationQuery(invalidLocationId);
            var subject = new SparePartStoragePlacesByLocationQueryValidator();

            var result = subject.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1, "only one rule is set");
        }
    }
}