using Ascentry.E2E.Contracts.PrecautionManagement;

namespace Ascentry.E2E.Contracts.Pages
{
    public interface IPrecautionManagementListPage
    {
        Task VerifyPrecautionTableAsync();
        Task<List<PatientDetailsRow>> GetPatientPrecautionsAsync(List<string> addedEvents, bool associateSpecimen);
    }
}
