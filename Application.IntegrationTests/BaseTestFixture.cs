using NUnit.Framework;
using Tournament.Application.IntegrationTests;

namespace Tournament.Application.IntegrationTests;

using static Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
       await ResetState();
    }
}

