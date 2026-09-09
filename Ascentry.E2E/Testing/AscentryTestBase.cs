
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Contracts.Layout;

namespace Ascentry.E2E.Testing
{
    public abstract class AscentryTestBase
    {
        protected ILoginPage LoginPage { get; }
        protected IHeader Header { get; }
        protected INavigationMenu NavigationMenu { get; }
        protected IGeneratePublicationPage GeneratePublicationPage { get; }
        protected IExpertRuleListPage ExpertRuleListPage { get; }
        protected IExpertRulePage ExpertRulePage { get; }
        private readonly AscentryFixture _fixture;

        protected AscentryTestBase(AscentryFixture fixture)
        {
            _fixture = fixture;
            LoginPage = fixture.LoginPage;
            Header = fixture.Header;
            NavigationMenu = fixture.NavigationMenu;
            GeneratePublicationPage = fixture.GeneratePublicationPage;
            ExpertRuleListPage = fixture.ExpertRuleListPage;
            ExpertRulePage = fixture.ExpertRulePage;
        }
    }
}
