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
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                // Exclude XUnit temporary assembly - causes exception.
                // Exclude test assembly.
                .Where(a => a.FullName?.Contains("DynamicProxyGenAssembly2", StringComparison.OrdinalIgnoreCase) is false &&
                            a.FullName?.Contains("ScanApp.tests", StringComparison.OrdinalIgnoreCase) is false)
                .ToArray();

            var implementors = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableTo(typeof(VersionedEntity)) && t != typeof(VersionedEntity))
                .Select(x => x.Name)
                .ToList();
            var configurations = typeof(VersionedEntityConfiguration<>)
                .GetDerivingTypes(assemblies)
                .Select(x =>
                {
                    var index = x.Name.LastIndexOf("configuration", StringComparison.OrdinalIgnoreCase);
                    return index >= 0
                        ? x.Name.Remove(index, "configuration".Length)
                        : x.Name;
                })
                .ToList();

            implementors.Should().BeEquivalentTo(configurations);
        }
    }
}