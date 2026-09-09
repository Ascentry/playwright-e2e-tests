using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Tests.Attributes;
using Xunit;

namespace Ascentry.E2E.Tests.Sandbox
{
    // Not necessary to use scroll to have the elements in the DOM as virtual scroll is not used for the list of publication events
    [Products(ProductEnum.InfectionTracker)]
    public class PublicationSandboxTests : AbstractTestBase
    {
        [Fact(DisplayName = "Get publication ORU events with  filter parameters: publication name, specimen number")]
        public async Task Should_Get_Generated_ORU_Event_With_PublicationName_And_SpecimenNumber()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Epidemiology.Publications.GeneratePublications);
            string specimenNumber = "100027471156";
            string publicationName = "Publication JMC workList ^*:\"/\\?|<>";
            var rows = await GeneratePublicationPage.GetPublicationEventRows(PublicationEventContextTypeEnum.ORU, publicationName, specimenNumber);
            Assert.Single(rows);
            Assert.Equal(specimenNumber, rows[0].SpecimenNumber);
        }

        [Fact(DisplayName = "Get publication ORU events with  filter parameters: publication name, specimen number, event name")]
        public async Task Should_Get_Generated_ORU_Event_With_PublicationName_SpecimenNumber_And_EventName()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Epidemiology.Publications.GeneratePublications);
            string specimenNumber = "100034883883";
            string publicationName = "Bulletin épidémiologique";
            string eventName = "BLSE";
            var rows = await GeneratePublicationPage.GetPublicationEventRows(PublicationEventContextTypeEnum.ORU, publicationName, specimenNumber, eventName);
            Assert.Single(rows);
            Assert.Equal(specimenNumber, rows[0].SpecimenNumber);
        }

        [Fact(DisplayName = "Get publication ORU events with  filter parameters: publication name, specimen number, event name, start date and end date of the studied period")]
        public async Task Should_Get_Generated_ORU_Events_With_All_parameters()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Epidemiology.Publications.GeneratePublications);
            string specimenNumber = "100034883883";
            string publicationName = "Bulletin épidémiologique";
            string eventName = "BLSE";
            string startDate = "01/06/2019 00:00";
            string endDate = "20/07/2026 00:00";
            var rows = await GeneratePublicationPage.GetPublicationEventRows(PublicationEventContextTypeEnum.ORU, publicationName, specimenNumber, eventName, startDate, endDate);
            Assert.Single(rows);
            Assert.Equal(specimenNumber, rows[0].SpecimenNumber);
        }

        [Fact(DisplayName = "Get publication ADT events with  filter parameters:  publication name, specimen number, event name, start date en end date of the search period")]
        public async Task Should_Get_Generated_ADT_Event_With_All_parameters()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Epidemiology.Publications.GeneratePublications);
            string patient = "LAC18529 Pau18529";
            string publicationName = "Bulletin épidémiologique";
            string eventName = "BLSE";
            string startDate = "01/06/2026 00:00";
            string endDate = "20/07/2026 00:00";
            var rows = await GeneratePublicationPage.GetPublicationEventRows(PublicationEventContextTypeEnum.ADT, publicationName, patient, eventName, startDate, endDate);
            Assert.Empty(rows);
        }
    }
}
