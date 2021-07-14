using FluentAssertions;
using ScanApp.Components.ScanAppTable.Options;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.Options
{  
    public class ScanAppTableOptionsTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new ScanAppTableOptions();

            subject.Should().BeOfType<ScanAppTableOptions>();
        }

        [Fact]
        public void Will_create_instance_without_parameters_and_toolbar_will_be_disabled_along_with_all_options()
        {
            var subject = new ScanAppTableOptions();

            subject.Should().BeOfType<ScanAppTableOptions>();
            Assert.False(subject.ShowToolBar);
            Assert.False(subject.AllowFiltering);
            Assert.False(subject.AllowGrouping);
        }

        [Fact]
        public void Will_create_instance_with_grouping_enabled_and_toolbar_enabled()
        {
            var subject = new ScanAppTableOptions()
            {
                AllowGrouping = true
            };

            subject.Should().BeOfType<ScanAppTableOptions>();
            Assert.True(subject.AllowGrouping);
            Assert.True(subject.ShowToolBar);
        }

        [Fact]
        public void Will_create_instance_with_filtering_enabled_and_toolbar_enabled()
        {
            var subject = new ScanAppTableOptions()
            {
                AllowFiltering = true
            };

            subject.Should().BeOfType<ScanAppTableOptions>();
            Assert.True(subject.AllowFiltering);
            Assert.True(subject.ShowToolBar);
        }

        [Fact]
        public void Will_create_instance_with_filtering_and_grouping_disabled_and_toolbar_disabled()
        {
            var subject = new ScanAppTableOptions()
            {
                AllowGrouping = false,
                AllowFiltering = false
            };

            subject.Should().BeOfType<ScanAppTableOptions>();
            Assert.False(subject.ShowToolBar);
        }
    }
}
