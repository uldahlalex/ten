using tests.Utilities;

namespace tests.Live;

public class IntegrationTests : ApiTestBase
{


    [Test]
    [Explicit]
    public Task Waits()
    {
        Console.WriteLine(App.Urls.First());
        Console.ReadLine();
        return Task.CompletedTask;
    }
}