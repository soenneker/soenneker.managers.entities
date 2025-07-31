using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Managers.Entities.Tests;

[Collection("Collection")]
public class EntitiesManagerTests : FixturedUnitTest
{
    public EntitiesManagerTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
