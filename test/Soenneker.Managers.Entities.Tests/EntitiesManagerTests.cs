using Soenneker.Tests.HostedUnit;

namespace Soenneker.Managers.Entities.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class EntitiesManagerTests : HostedUnitTest
{
    public EntitiesManagerTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
