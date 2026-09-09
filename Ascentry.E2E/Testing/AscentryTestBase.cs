
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Contracts.Layout;
using Ascentry.E2E.Contracts.Pages;
using Ascentry.E2E.Pages;

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
        protected IPatientRecordPage PatientRecordPage { get; }
        protected IPrecautionManagementListPage PrecautionManagementListPage { get; }

        protected AscentryTestBase(AscentryFixture fixture)
        {
            LoginPage = fixture.LoginPage;
            Header = fixture.Header;
            NavigationMenu = fixture.NavigationMenu;
            GeneratePublicationPage = fixture.GeneratePublicationPage;
            ExpertRuleListPage = fixture.ExpertRuleListPage;
            ExpertRulePage = fixture.ExpertRulePage;
            PatientRecordPage = fixture.PatientRecordPage;
            PrecautionManagementListPage = fixture.PrecautionManagementListPage;
        }
    }
}
