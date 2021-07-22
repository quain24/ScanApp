using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Application.SpareParts.Commands.CreateSpareParts
{
    public class CreateSparePartsCommandHandlerTests : SqlLiteInMemoryDbFixture
    {
        public ITestOutputHelper Output { get; }

        public CreateSparePartsCommandHandlerTests(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}