using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandValidatorTests
    {
        [Fact]
        public void Validates_proper_command()
        {
            var spareParts = new SparePartModel[]
            {
                new ("name_a", 1, "art_id", "place_id"),
                new ("name_b", 10, "art_id_2", "place_id")
            };
            var command = new CreateSparePartsCommand(spareParts);
            var sut = new CreateSparePartsCommandValidator();

            var result = sut.Validate(command);
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Does_not_allow_null_collection_in_command()
        {
            var command = new CreateSparePartsCommand(null);
            var sut = new CreateSparePartsCommandValidator();

            var result = sut.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1)
                .And.Subject.First().ErrorMessage.Should().Be("Instead of spare parts collection a null was passed inside command.");
        }

        [Fact]
        public void Does_not_allow_empty_collection_in_command()
        {
            var command = new CreateSparePartsCommand(Array.Empty<SparePartModel>());
            var sut = new CreateSparePartsCommandValidator();

            var result = sut.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1)
                .And.Subject.First().ErrorMessage.Should().Be("There are no spare parts to be added to database, command is invalid");
        }

        [Fact]
        public void Does_not_allow_any_spare_part_model_in_command_to_be_null()
        {
            var spareParts = new SparePartModel[]
            {
                new ("name_a", 1, "art_id", "place_id"),
                new ("name_b", 10, "art_id_2", "place_id"),
                null
            };
            var command = new CreateSparePartsCommand(spareParts);
            var sut = new CreateSparePartsCommandValidator();

            var result = sut.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            var error = result.Errors.First();
            error.ErrorMessage.Should().Be("One of passed spare parts is null, aborting!");
            error.PropertyName.Should().Be(nameof(command.SpareParts) + "[2]", "spare part model with index 2 is null");
        }

        public static TheoryData<List<SparePartModel>> WrongAmountModels =>
            CreateSparePartsCommandValidatorTestsData.WrongAmountSparePartModel;

        [Theory]
        [MemberData(nameof(WrongAmountModels))]
        public void Every_spare_part_model_model_amount_must_be_between_min_and_max(List<SparePartModel> sparePartModels)
        {
            var command = new CreateSparePartsCommand(sparePartModels.ToArray());
            var sut = new CreateSparePartsCommandValidator();

            var result = sut.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(sparePartModels.Count(s => s.Amount is <= 0 or > 1000));
            using var scope = new AssertionScope();
            result.Errors.ForEach(e => e.ErrorMessage.Should().Be("Spare part quantity must be between 1 and 1000"));
        }
    }
}