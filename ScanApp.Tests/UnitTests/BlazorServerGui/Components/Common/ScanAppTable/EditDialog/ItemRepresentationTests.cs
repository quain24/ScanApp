using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ScanApp.Components.Common.ScanAppTable.EditDialog;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.EditDialog
{
    public class ItemRepresentationTests
    {
        
        [Fact]
        public void Will_create_instance()
        {
            var fixture = new ItemRepresentationFixture("test", 20, 20, DateTime.Now, 20.20);
            var properties = fixture.GetType().GetProperties();
            var subject = new ItemRepresentation<ItemRepresentationFixture>(properties, fixture);

            subject.Should().BeOfType<ItemRepresentation<ItemRepresentationFixture>>();
        }

        [Fact]
        public void Will_create_instance_and_bind_values_correctly()
        {
            var fixture = new ItemRepresentationFixture("test", 20, 20, new DateTime(1,1,1), 20.20);
            var properties = fixture.GetType().GetProperties();
            var subject = new ItemRepresentation<ItemRepresentationFixture>(properties, fixture);

            Assert.Equal("test", subject.Strings[0]); 
            Assert.Equal(20, subject.Ints[1]); 
            Assert.Equal(20, subject.Decimals[2]); 
            Assert.Equal(new DateTime(1,1,1), subject.DateTimes[3]); 
            Assert.Equal(20.20, subject.Doubles[4]);
        }

        [Fact]
        public void Will_create_instance_with_null_string_and_will_create_string_empty_in_place()
        {
            var fixture = new ItemRepresentationFixture(null, 20, 20, new DateTime(1,1,1), 20.20);
            var properties = fixture.GetType().GetProperties();
            var subject = new ItemRepresentation<ItemRepresentationFixture>(properties, fixture);

            Assert.Equal(string.Empty, subject.Strings[0]);
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_properties()
        {
            var fixture = new ItemRepresentationFixture("test", 20, 20, new DateTime(1,1,1), 20.20);
            Action act = () => _ = new ItemRepresentation<ItemRepresentationFixture>(null, fixture);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_throw_arg_null_exc_if_not_given_item()
        {
            var fixture = new ItemRepresentationFixture("test", 20, 20, new DateTime(1,1,1), 20.20);
            var properties = fixture.GetType().GetProperties();
            Action act = () => _ = new ItemRepresentation<ItemRepresentationFixture>(properties, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Will_correctly_save_changes()
        {
            var fixture = new ItemRepresentationFixture("test", 20, 20, new DateTime(1,1,1), 20.20);
            var properties = fixture.GetType().GetProperties();
            var subject = new ItemRepresentation<ItemRepresentationFixture>(properties, fixture);

            subject.Strings[0] = "testchange";
            subject.Ints[1] = 1;
            subject.Decimals[2] = 1;
            subject.DateTimes[3] = new DateTime(2, 2, 2);
            subject.Doubles[4] = 1.1;
            subject.SaveChanges();

            Assert.Equal("testchange", subject.Objects[0]);
            Assert.Equal(1, subject.Objects[1]);
            Assert.Equal(new decimal(1), subject.Objects[2]);
            Assert.Equal(new DateTime(2, 2, 2), subject.Objects[3]);
            Assert.Equal(1.1, subject.Objects[4]);
        }
    }
}
