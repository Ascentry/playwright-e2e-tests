using Ascentry.E2E.Configurations;
using Ascentry.E2E.Testing;
using Microsoft.Extensions.Configuration;

namespace InfectionTracker.E2E.Tests
{
    public abstract class AbstractTestBase : AscentryTestBase
    {
        static AbstractTestBase()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("configuration.json", optional: true)
                .AddUserSecrets<AbstractTestBase>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            TestConfiguration.Initialize(configuration);
        }
    }
}
