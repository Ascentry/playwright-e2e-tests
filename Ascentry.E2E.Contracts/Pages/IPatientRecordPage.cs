using Ascentry.E2E.Contracts.PatientRecord;

namespace Ascentry.E2E.Contracts.Pages
{
    public interface IPatientRecordPage
    {
        Task SearchPatientAsync(PatientRecordFilter filter);
        Task<PatientDetails> GetSelectedPatientDetailsAsync();
        Task<PatientStayDetails> GetSelectedPatientStayDetailsAsync();
        Task<PatientSpecimenDetails> GetSelectedSpecimenDetailsAsync();
        Task<IInfectionMonitoringListPage> AddSelectedPatientToInfectionMonitoringAsync(bool associateSpecimen);
        Task<IPrecautionManagementListPage> AddSelectedPatientToPrecautionManagementAsync(List<string> addedEvents, bool associateSpecimen);
        Task VerifyPatientNameAsync();
        Task<bool> PatientPrecautionsAsync();
        Task<bool> PatientHasInfectionsAsync();
    }
}
