using FluentAssertions;
using ScanApp.Common.Extensions;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence.Configurations;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.IntegrationTests.Domain.Entities
{
    public class VersionedEntityTests
    {
        [Fact]
        public void All_entities_derived_from_VersionedEntity_have_proper_configurations()
        {
            var implementors = AppDomain.CurrentDomain
                .GetAssemblies()
                // Ignore temporary XUnit assembly.
                .Where(a => a.GetName().Name?.Contains("DynamicProxyGenAssembly2", StringComparison.OrdinalIgnoreCase) is false)
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableTo(typeof(VersionedEntity)) && t != typeof(VersionedEntity))
                .ToList();
            var configurations = typeof(VersionedEntityConfiguration<>).GetDerivingTypes()
                .SelectMany(t => t.BaseType?.GenericTypeArguments);

            implementors.Should().BeEquivalentTo(configurations);
        }
    }
}