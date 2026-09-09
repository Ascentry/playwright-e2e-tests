# Core.Workflows

## Purpose

This folder contains **high-level business workflows** exposed by the framework.

A workflow orchestrates multiple pages, layouts, components, and user actions to achieve a complete business objective. It encapsulates implementation details and provides a simplified API for consumer test projects.

## Design Principle

A workflow belongs in this folder if it:

- Represents a complete business scenario.
- Spans multiple pages or application areas.
- Combines several user interactions into a single reusable operation.
- Simplifies test implementation by hiding low-level automation details.

Typical examples include:

- `AuthenticationWorkflow`
- `PublicationWorkflow`
- `PatientCreationWorkflow`
- `ExpertRuleExecutionWorkflow`

## Why Use Workflows?

Without workflows, tests often become verbose and tightly coupled to the application's UI structure.

### Without a workflow

```csharp
await NavigationMenu.GoToAsync(
    InfectionTrackerMenu.Settings.Expertise.ExpertRules);

var expertRule = await ExpertRuleListPage.OpenAsync(
    "101309-Générer un évènement");

await expertRule.VerifyEditModeAsync();

var result = await expertRule.