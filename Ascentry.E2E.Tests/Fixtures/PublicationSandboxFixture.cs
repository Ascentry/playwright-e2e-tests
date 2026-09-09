using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Testing;

namespace Ascentry.E2E.Tests.Fixtures
{
    public class PublicationSandboxFixture : AscentryFixture
    {
        public override async ValueTask InitializeAsync()
        {
            await base.InitializeAsync();

            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Epidemiology.Publications.GeneratePublications);
        }
    }
}
