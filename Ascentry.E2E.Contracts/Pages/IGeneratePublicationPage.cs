using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.Publications;

namespace Ascentry.E2E.Contracts.Interfaces
{
    public interface IGeneratePublicationPage
    {
        Task<List<PublicationEventDetailsRow>> GetPublicationEventRows(PublicationEventContextTypeEnum eventContextType, string publicationName, string eventContextIdentifier, string eventName = null, string startDate = null, string endDate = null);
    }
}
