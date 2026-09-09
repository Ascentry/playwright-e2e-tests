using Ascentry.E2E.Contracts.PatientRecord;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Testing;
using Xunit;

namespace Ascentry.E2E.Tests.Sandbox
{
    public class PrecautionManagementTests : AscentryTestBase, IClassFixture<AscentryFixture>
    {
        public PrecautionManagementTests(AscentryFixture fixture) : base(fixture)
        {
        }

        [Fact(DisplayName = "Add a patient to precaution management with specimen")]
        public async Task Add_Patient_To_Precaution_Management_With_Specimen()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Patient.RecordsManagement.PatientRecord);

            var filter = new PatientRecordFilter() { PatientLastName = "AAD9814" };
            List<string> addedEvents = ["BLSE", "Clostridium"];

            await PatientRecordPage.SearchPatientAsync(filter);

            PatientDetails patientDetails =  await PatientRecordPage.GetSelectedPatientDetailsAsync();
            PatientStayDetails patientStayDetails = await PatientRecordPage.GetSelectedPatientStayDetailsAsync();
            PatientSpecimenDetails selectedSpecimenDetails = await PatientRecordPage.GetSelectedSpecimenDetailsAsync();

            var precautionManagementListPage = await PatientRecordPage.AddSelectedPatientToPrecautionManagementAsync(addedEvents, true);
            var patientPrecautionRows = await precautionManagementListPage.GetPatientPrecautionsAsync(addedEvents, true);

            Assert.Single(patientPrecautionRows);
            //Assert.Equal(patientStayDetails.AdmissionDate, patientPrecautionRows[0].AdmissionDate);
            //Assert.Equal(patientStayDetails.ReleaseDate, patientPrecautionRows[0].ReleaseDate);
            Assert.Equal(patientDetails.PatientLastName, patientPrecautionRows[0].PatientLastName);
            Assert.Equal(patientDetails.PatientFirstName, patientPrecautionRows[0].PatientFirstName);
            Assert.Equal(patientDetails.Pids[0], patientPrecautionRows[0].Pids[0]);
            Assert.Equal(addedEvents.Count, patientPrecautionRows[0].Precautions.Count);

            foreach (var addedEvent in addedEvents)
            {
                var precautionEvents = patientPrecautionRows[0].Precautions.Select(x => x.EventName);
                Assert.Contains(addedEvent, precautionEvents);
            }

            Assert.Equal(selectedSpecimenDetails.SampleType, patientPrecautionRows[0].Precautions[0].SampleType);

            foreach (var precaution in patientPrecautionRows[0].Precautions)
            {
                Assert.Equal(selectedSpecimenDetails.SpecimenCollectionDate, precaution.SpecimenCollectionDate);
                Assert.Equal(selectedSpecimenDetails.SpecimenNumber, precaution.SpecimenNumber);
                Assert.Equal(selectedSpecimenDetails.SpecimenCareUnit, precaution.EventLocation);
                Assert.Empty(precaution.PrecautionType);
                Assert.Empty(precaution.PrecautionStartDate);
                Assert.Empty(precaution.PrecautionDuration);
                Assert.Empty(precaution.PrecautionEndDate);
            }

            //Assert.Equal(QuestionnaireStatusEnum.Unassociated, infectionDetailsRows[0].QuestionnaireStatus);
        }


        [Fact(DisplayName = "Add a patient to precaution management without specimen")]
        public async Task Add_Patient_To_Precaution_Management_Without_Specimen()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Patient.RecordsManagement.PatientRecord);

            var filter = new PatientRecordFilter() { PatientLastName = "AAD9814" };
            List<string> addedEvents = ["BLSE", "Clostridium"];

            await PatientRecordPage.SearchPatientAsync(filter);

            PatientDetails patientDetails = await PatientRecordPage.GetSelectedPatientDetailsAsync();
            PatientStayDetails patientStayDetails = await PatientRecordPage.GetSelectedPatientStayDetailsAsync();
            var precautionManagementListPage = await PatientRecordPage.AddSelectedPatientToPrecautionManagementAsync(addedEvents, false);
            var patientPrecautionRows = await precautionManagementListPage.GetPatientPrecautionsAsync(addedEvents, false);

            Assert.Single(patientPrecautionRows);
            //Assert.Equal(patientStayDetails.AdmissionDate, patientPrecautionRows[0].AdmissionDate);
            //Assert.Equal(patientStayDetails.ReleaseDate, patientPrecautionRows[0].ReleaseDate);
            Assert.Equal(patientDetails.PatientLastName, patientPrecautionRows[0].PatientLastName);
            Assert.Equal(patientDetails.PatientFirstName, patientPrecautionRows[0].PatientFirstName);
            Assert.Equal(patientDetails.Pids[0], patientPrecautionRows[0].Pids[0]);
            Assert.Equal(addedEvents.Count, patientPrecautionRows[0].Precautions.Count);

            foreach (var addedEvent in addedEvents)
            {
                var precautionEvents = patientPrecautionRows[0].Precautions.Select(x => x.EventName);
                Assert.Contains(addedEvent, precautionEvents);
            }

            foreach (var precaution in patientPrecautionRows[0].Precautions)
            {
                Assert.Empty(precaution.SpecimenCollectionDate);
                Assert.Empty(precaution.SpecimenNumber);
                Assert.Equal(patientStayDetails.PatientStayCareUnit, precaution.EventLocation);
                Assert.Empty(precaution.PrecautionType);
                Assert.Empty(precaution.PrecautionStartDate);
                Assert.Empty(precaution.PrecautionDuration);
                Assert.Empty(precaution.PrecautionEndDate);
            }
        }
    }
}
