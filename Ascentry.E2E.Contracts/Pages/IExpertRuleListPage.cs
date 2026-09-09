namespace Ascentry.E2E.Contracts.Interfaces
{
    public interface IExpertRuleListPage
    {
        Task<IExpertRulePage> OpenAsync(string expertruleName);
    }
}
