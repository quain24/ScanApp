using ScanApp.Application.SpareParts.Commands.CreateSpareParts;
using System.Collections.Generic;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    internal class CreateSparePartsCommandValidatorTestsData
    {
        public static TheoryData<List<SparePartModel>> WrongAmountSparePartModel
        {
            get
            {
                var data = new TheoryData<List<SparePartModel>>();

                data.Add(new List<SparePartModel>
                {
                    new SparePartModel("name_a", 1, "art_id", "place_id"),
                    new SparePartModel("name_b", 0, "art_id", "place_id"),
                    new SparePartModel("name_c", -1, "art_id", "place_id")
                });
                data.Add(new List<SparePartModel>
                {
                    new SparePartModel("name_a", 0, "art_id", "place_id"),
                    new SparePartModel("name_b", 10, "art_id", "place_id"),
                    new SparePartModel("name_c", 11, "art_id", "place_id")
                });
                data.Add(new List<SparePartModel>
                {
                    new SparePartModel("name_a", 1100, "art_id", "place_id"),
                    new SparePartModel("name_b", 10, "art_id", "place_id"),
                    new SparePartModel("name_c", -11, "art_id", "place_id")
                });
                data.Add(new List<SparePartModel>
                {
                    new SparePartModel("name_a", -1, "art_id", "place_id")
                });

                return data;
            }
        }
    }
}