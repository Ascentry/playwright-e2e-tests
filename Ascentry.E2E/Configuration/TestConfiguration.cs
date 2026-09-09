using Microsoft.Extensions.Configuration;

namespace Ascentry.E2E.Configurations
{
    public static class TestConfiguration
    {
        public static PlaywrightSettings Settings { get; private set; } = new PlaywrightSettings();

        public static void Initialize(IConfiguration configuration)
        {
            configuration
                .GetSection("Playwright")
                .Bind(Settings);
        }
    }
}