using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    internal class CreateSparePartsCommandValidatorTestsData
    {
        public static TheoryData<List<SparePartModel>> WrongAmountSparePartModels =>
            new()
            {
                new List<SparePartModel>
                {
                    new("name_a", 1, "art_id", "place_id"),
                    new("name_b", 0, "art_id", "place_id"),
                    new("name_c", -1, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 0, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new("name_c", 11, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 1100, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new("name_c", -11, "art_id", "place_id")
                },
                new List<SparePartModel> { new("name_a", -1, "art_id", "place_id") }
            };

        public static TheoryData<List<SparePartModel>> MissingNameSparePartModels =>
            new()
            {
                new List<SparePartModel>
                {
                    new("name_a", 1, "art_id", "place_id"),
                    new("    ", 2, "art_id", "place_id"),
                    new(string.Empty, 3, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 1, "art_id", "place_id"),
                    new(string.Empty, 10, "art_id", "place_id"),
                    new("name_c", 11, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 1000, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new(null, 11, "art_id", "place_id")
                },
                new List<SparePartModel> { new(string.Empty, 1, "art_id", "place_id") }
            };

        public static TheoryData<List<SparePartModel>> MissingArticleIdSparePartModels =>
            new()
            {
                new List<SparePartModel>
                {
                    new("name_a", 1, " ", "place_id"),
                    new("name_b", 2, string.Empty, "place_id"),
                    new("name_c", 3, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 1, null, "place_id"),
                    new("name_b", 10, string.Empty, "place_id"),
                    new("name_c", 11, "art_id", "place_id")
                },
                new List<SparePartModel>
                {
                    new("name_a", 1000, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new("name_c", 11, null, "place_id")
                },
                new List<SparePartModel> { new("name_a", 1, string.Empty, "place_id") }
            };

        public static TheoryData<List<SparePartModel>> MissingStoragePlaceSparePartModels =>
            new()
            {
                new List<SparePartModel>
                {
                    new("name_a", 1, "art_id", "place_id"),
                    new("name_b", 10, "art_id", string.Empty),
                    new("name_c", 11, "art_id", null)
                },
                new List<SparePartModel>
                {
                    new("name_a", 2, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new("name_c", 11, "art_id", string.Empty)
                },
                new List<SparePartModel>
                {
                    new("name_a", 300, "art_id", "place_id"),
                    new("name_b", 10, "art_id", "place_id"),
                    new("name_c", 11, "art_id", null)
                },
                new List<SparePartModel> { new("name_a", 1, "art_id", "") }
            };

        public static TheoryData<List<SparePartModel>, int> CombinedDifferentErrorsSparePartsModels =>
            new()
            {
                {
                    new List<SparePartModel>
                    {
                        new("", 1, "art_id", ""),
                        new("name_b", 0, "art_id", "place_id"),
                        new("name_c", -1, "art_id", null)
                    },
                    5
                },
                {
                    new List<SparePartModel>
                    {
                        new("", 0, "art_id", "place_id"),
                        new("name_b", 10, "", "place_id"),
                        new("name_c", 11, "art_id", null)
                    },
                    4
                },
                {
                    new List<SparePartModel>
                    {
                        new("name_a", 1, "art_id", string.Empty),
                        new("name_b", 10, "art_id", "place_id"),
                        new("name_c", -11, "art_id", "place_id")
                    },
                    2
                },
                { null, 1 }
            };
    }
}