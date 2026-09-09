using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.InfectionMonitoring;
using Ascentry.E2E.Contracts.PatientRecord;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Testing;
using Ascentry.E2E.Tests.Attributes;
using Xunit;

namespace Ascentry.E2E.Tests.Sandbox
{

    [Products(ProductEnum.InfectionTracker)]
    public class InfectionMonitoringTests : AscentryTestBase, IClassFixture<AscentryFixture>
    {
        public InfectionMonitoringTests(AscentryFixture fixture) : base(fixture)
        {
        }

        [Fact(DisplayName = "Add a patient to infection monitoring with specimen")]
        public async Task Add_Patient_To_Infection_Monitoring_With_Specimen()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Patient.RecordsManagement.PatientRecord);

            var patientRecordFilter = new PatientRecordFilter() { PatientLastName = "AAD9814" };
            await PatientRecordPage.SearchPatientAsync(patientRecordFilter);

            PatientSpecimenDetails selectedSpecimenDetails = await PatientRecordPage.GetSelectedSpecimenDetailsAsync();

            var infectionMonitoringListPage = await PatientRecordPage.AddSelectedPatientToInfectionMonitoringAsync(true);
            var infectionFilter = new InfectionFilter()
            {
                PatientLastName = "AAD9814"
            };

            List<InfectionDetailsRow> infectionDetailsRows = await infectionMonitoringListPage.GetInfectionsAsync(infectionFilter);

            Assert.NotEmpty(infectionDetailsRows);
            Assert.NotEmpty(infectionDetailsRows[0].PatientFirstName);
            Assert.Equal(selectedSpecimenDetails.SampleType, infectionDetailsRows[0].SampleType);
            Assert.Equal(selectedSpecimenDetails.SpecimenCollectionDate, infectionDetailsRows[0].SpecimenCollectionDate);
            Assert.Equal(selectedSpecimenDetails.SpecimenCareUnit, infectionDetailsRows[0].CareUnit);
            Assert.Empty(infectionDetailsRows[0].InfectionType);
            Assert.Equal("Présumée", infectionDetailsRows[0].InfectionStatus);
            Assert.Empty(infectionDetailsRows[0].InfectionQualification);
            Assert.True(infectionDetailsRows[0].HasEmptyComments);
            Assert.Equal(QuestionnaireStatusEnum.Unassociated, infectionDetailsRows[0].QuestionnaireStatus);

            var newPatientRecordPage = await infectionMonitoringListPage.GoToPatientRecordAsync("AAD9814", 0);
            bool hasInfections = await newPatientRecordPage.PatientHasInfectionsAsync();

            Assert.True(hasInfections);
        }


        [Fact(DisplayName = "Add a patient to infection monitoring without specimen")]
        public async Task Add_Patient_To_Infection_Monitoring_Without_Specimen()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Patient.RecordsManagement.PatientRecord);

            var patientRecordFilter = new PatientRecordFilter() { PatientLastName = "AAD9814" };
            await PatientRecordPage.SearchPatientAsync(patientRecordFilter);

            var infectionMonitoringListPage = await PatientRecordPage.AddSelectedPatientToInfectionMonitoringAsync(false);
            var infectionFilter = new InfectionFilter()
            {
                PatientLastName = "AAD9814"
            };

            List<InfectionDetailsRow> infectionDetailsRows = await infectionMonitoringListPage.GetInfectionsAsync(infectionFilter);

            Assert.NotEmpty(infectionDetailsRows);
            Assert.NotEmpty(infectionDetailsRows[0].PatientFirstName);
            Assert.Empty(infectionDetailsRows[0].SampleType);
            Assert.Empty(infectionDetailsRows[0].SpecimenCollectionDate);
            Assert.NotEmpty(infectionDetailsRows[0].CareUnit);
            Assert.Empty(infectionDetailsRows[0].InfectionType);
            Assert.Equal("Présumée", infectionDetailsRows[0].InfectionStatus);
            Assert.Empty(infectionDetailsRows[0].InfectionQualification);
            Assert.True(infectionDetailsRows[0].HasEmptyComments);
            Assert.Equal(QuestionnaireStatusEnum.Unassociated, infectionDetailsRows[0].QuestionnaireStatus);

            var newPatientRecordPage = await infectionMonitoringListPage.GoToPatientRecordAsync("AAD9814", 0);
            bool hasInfections = await newPatientRecordPage.PatientHasInfectionsAsync();

            Assert.True(hasInfections);
        }

    }
}
