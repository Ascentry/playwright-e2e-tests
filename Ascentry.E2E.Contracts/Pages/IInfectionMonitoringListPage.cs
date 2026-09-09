using Ascentry.E2E.Contracts.InfectionMonitoring;

namespace Ascentry.E2E.Contracts.Pages
{
    public interface IInfectionMonitoringListPage
    {
        Task<List<InfectionDetailsRow>> GetInfectionsAsync(InfectionFilter filter);
        Task<IPatientRecordPage> GoToPatientRecordAsync(string patientLastName, int rowIndex);
    }
}
