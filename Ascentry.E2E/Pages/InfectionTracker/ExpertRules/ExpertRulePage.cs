using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.ExpertRules;
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Enums;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.ExpertRules
{
    internal class ExpertRulePage : PageBase, IExpertRulePage
    {
        internal ExpertRulePage(IPage page) : base(page) { }

        public async Task<bool> VerifyEditModeAsync()
        {
            string expectedTitle = GetLabel(TranslationEnum.ModificationOfAnExpertRule);
            var title = Page.Locator("byg-title", new PageLocatorOptions() { HasText = expectedTitle });
            await title.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });

            return await title.IsVisibleAsync();
        }

        /// <summary>
        /// Execute expert rule in test mode
        /// </summary>
        /// <param name="contextType">e.g specimen number, patient pid</param>
        /// <param name="contextValue">value of specimen number, patient pid depending on the context type</param>
        /// <returns>Contains the results produced by an expert rule for each rule row/returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<ExpertRuleResult> ExecuteTestModeAsync(ExpertRuleTestModeContextTypeEnum contextType, string contextValue)
        {
            var testModeItem = Page.Locator("byg-form-switch-mode div.right");
            await testModeItem.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
            await testModeItem.ClickAsync();

            ILocator contextInput, inputLabel;

            switch (contextType)
            {
                case ExpertRuleTestModeContextTypeEnum.SpecimenNumber:
                    {
                        var specimenNumberLabel = GetLabel(TranslationEnum.SampleNumber);
                        inputLabel = Page.Locator($"byg-form-text span:has-text('{specimenNumberLabel}')");
                        contextInput = Page.Locator($"byg-form-text[name='specimenNo'] input[type='text']");
                        break;
                    }
                case ExpertRuleTestModeContextTypeEnum.PatientPid:
                    {
                        var pidLabel = GetLabel(TranslationEnum.Pid);
                        inputLabel = Page.Locator($"byg-form-text span:has-text('{pidLabel}')");
                        contextInput = Page.Locator($"byg-form-text[name='PID'] input[type='text']");
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException($"{contextType} is not a supported value for {nameof(ExpertRuleTestModeContextTypeEnum)}");
                    }
            }

            await inputLabel.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
            await contextInput.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });

            if (contextInput != null)
            {
                await contextInput.FillAsync(contextValue);
            }

            var validateButton = Page.Locator($"nina-expert-rule-menu byg-button:has(label:has-text('{GetLabel(TranslationEnum.Validate)}'))");
            await Assertions.Expect(validateButton).ToBeEnabledAsync();
            await validateButton.ClickAsync();

            // Wait for the response in order to have the check or clear icons visible
            await Page.RunAndWaitForResponseAsync(
                async () =>
                {
                    await validateButton.ClickAsync();
                },
                r => r.Url.Contains("api/expertRule/rule/simulate") && r.Status == 200);

            return await GetRuleResult();
        }

        private async Task<ExpertRuleResult> GetRuleResult()
        {
            var rowBlocks = Page.Locator("nina-expert-rule-row .blocks");
            int rowBlockCount = await rowBlocks.CountAsync();
            var ruleResult = new ExpertRuleResult
            {
                RowResults = new List<ExpertRuleRowResult>(rowBlockCount)
            };

            for (int i = 0; i < rowBlockCount; i++)
            {
                var rowBlock = rowBlocks.Nth(i);
                ruleResult.RowResults.Add(await GetRowBlockResult(rowBlock));
            }

            return ruleResult;
        }

        private static async Task<ExpertRuleRowResult> GetRowBlockResult(ILocator rowBlock)
        {
            var conditionBlocks = rowBlock.Locator("nina-expert-rule-condition");
            int conditionBlockCount = await conditionBlocks.CountAsync();
            var rowResult = new ExpertRuleRowResult()
            {
                RowConditionResults = new List<ExpertRuleRowConditionResult>(conditionBlockCount)
            };

            for (int i = 0; i < conditionBlockCount; i++)
            {
                var conditionBlock = conditionBlocks.Nth(i);
                rowResult.RowConditionResults.Add(await GetConditionBlockResult(conditionBlock));
            }

            rowResult.IsSuccess = rowResult.RowConditionResults.Count != 0 && rowResult.RowConditionResults.All(x => x.IsSuccess); // Conditions blocks are necessarily linked by AND operator;
            return rowResult;
        }

        private static async Task<ExpertRuleRowConditionResult> GetConditionBlockResult(ILocator conditionBlock)
        {
            var instructionBlocks = conditionBlock.Locator("nina-expert-rule-instruction");
            int instructionBlockCount = await instructionBlocks.CountAsync();
            var conditionResult = new ExpertRuleRowConditionResult
            {
                RowInstructionResults = new List<ExpertRuleRowInstructionResult>(instructionBlockCount)
            };

            for (int i = 0; i < instructionBlockCount; i++)
            {
                var instructionBlock = instructionBlocks.Nth(i);
                conditionResult.RowInstructionResults.Add(await GetInstructionBlockResult(instructionBlock));
            }

            var conditionOperators = conditionBlock.Locator("span.instruction-operator");
            int operatorCount = await conditionOperators.CountAsync();
            var operatorValues = new List<ExpertRuleOperatorEnum>();

            for (int i = 0; i < operatorCount; i++)
            {
                string operatorText = await conditionOperators.Nth(i).InnerTextAsync();

                var andLabel = GetLabel(TranslationEnum.And);
                var orLabel = GetLabel(TranslationEnum.Or);

                if (!string.IsNullOrEmpty(operatorText))
                {
                    continue;
                }

                operatorText = operatorText.Trim().ToLower();

                if (operatorText.Equals(andLabel.ToLower()))
                {
                    operatorValues.Add(ExpertRuleOperatorEnum.And);
                }

                if (operatorText.Equals(orLabel.ToLower()))
                {
                    operatorValues.Add(ExpertRuleOperatorEnum.Or);
                }
            }

            bool onlyAndConditions = operatorValues.Count != 0 && operatorValues.All(x => x == ExpertRuleOperatorEnum.And);

            var checkIcons = conditionBlock.Locator("nina-expert-rule-instruction byg-icon[icon='check']");
            int checkIconCount = await checkIcons.CountAsync();

            if (onlyAndConditions)
            {
                conditionResult.IsSuccess = checkIconCount == instructionBlockCount;
            }
            else
            {
                conditionResult.IsSuccess = checkIconCount > 0; // At least one OR condition if several instructions in the condition block or if it's a mono-instruction condition
            }

            return conditionResult;
        }

        private static async Task<ExpertRuleRowInstructionResult> GetInstructionBlockResult(ILocator instructionBlock)
        {
            var checkIcon = instructionBlock.Locator("byg-icon[icon='check']");
            var instructionResult = new ExpertRuleRowInstructionResult
            {
                IsSuccess = await checkIcon.IsVisibleAsync()
            };

            if (!instructionResult.IsSuccess)
            {
                var clearIcon = instructionBlock.Locator("byg-icon[icon='clear']");
                await clearIcon.IsVisibleAsync();
            }

            string instructionLabel = await instructionBlock.Locator("div.instruction span").InnerTextAsync();
            instructionResult.InstructionLabel = instructionLabel.Trim();

            return instructionResult;

        }
    }
}
