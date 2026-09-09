using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.ExpertRules;

namespace Ascentry.E2E.Contracts.Interfaces
{
    public interface IExpertRulePage
    {
        Task<bool> VerifyEditModeAsync();
        Task<ExpertRuleResult> ExecuteTestModeAsync(ExpertRuleTestModeContextTypeEnum contextType, string contextValue);
    }
}
