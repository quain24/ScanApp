using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.EditDialog;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.EditDialog
{
    public class ItemClonerTests
    {
        [Fact]
        public void Will_clone_string()
        {
            var testString = "testString";
            var testStringCloned = ItemCloner.Clone(testString);

            Assert.Equal(testString,testStringCloned);
        }

        [Fact]
        public void Will_clone_collection_of_string()
        {
            var testList = new List<string>();
            testList.Add("2");
            testList.Add("test");
            testList.Add("test2");

            var testListCloned = ItemCloner.Clone(testList);
            Assert.Equal(testList, testListCloned);
        }

        [Fact]
        public void Will_clone_nested_object()
        {
            var testObject = new ItemClonerFixture();
            testObject.Id = 1;
            testObject.Fixture = new ItemClonerFixture();
            testObject.Fixture.Id = 2;
            testObject.Fixture.Fixture = new ItemClonerFixture();
            testObject.Fixture.Fixture.Id = 3;

            var testObjectCloned = ItemCloner.Clone(testObject);
            Assert.True(ItemsAreIdentical(testObjectCloned, testObject));
        }

        private bool ItemsAreIdentical(ItemClonerFixture item1, ItemClonerFixture item2)
        {
            if (item1.Fixture is not null && item2.Fixture is not null)
            {
                if (!ItemsAreIdentical(item1.Fixture, item2.Fixture))
                {
                    return false;
                }
            }
            if (item1.Id == item2.Id)
            {
                return true;
            }
            return false;
        }
    }
}
