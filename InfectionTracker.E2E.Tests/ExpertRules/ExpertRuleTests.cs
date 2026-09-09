using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.ExpertRules;
using Ascentry.E2E.Navigations.Menus;

namespace InfectionTracker.E2E.Tests.ExpertRules
{
    public class ExpertRuleTests: AbstractTestBase
    {

        [Fact(DisplayName = "Execute an expert rule in test mode - Success - one row with one condition with one instruction")]
        public async Task Should_Execute_Test_Mode_One_Row_One_Condition_One_Instruction()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Settings.Expertise.ExpertRules);

            var expertRule = await ExpertRuleListPage.OpenAsync("101309-Générer un évènement");
            await expertRule.VerifyEditModeAsync();

            ExpertRuleResult result = await expertRule.ExecuteTestModeAsync(ExpertRuleTestModeContextTypeEnum.PatientPid, "P11161287623");
            Assert.Single(result.RowResults);
            Assert.True(result.RowResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].RowInstructionResults[0].IsSuccess);
        }
    }
}
