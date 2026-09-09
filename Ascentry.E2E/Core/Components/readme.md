# Core.Components

## Purpose

This folder contains **internal reusable UI components** used by the framework implementation.

These components provide common functionality shared across pages, workflows, layouts, and other framework features, but they are **not intended to be part of the public API** and should not be used directly by consumer test projects.

## Design Principle

A component belongs in this folder if it:

- Represents a reusable technical UI building block.
- Is used internally by the framework.
- Is considered an implementation detail of the automation layer.
- Does not need to be accessed directly by consumer tests.

Typical examples include:

- `SpinnerComponent`
- `DialogComponent`
- `DataGridComponent`
- `DatePickerComponent`
- `SearchComponent`
- `NotificationComponent`

## What Does Not Belong Here

Components that are part of the framework's public API should be placed in dedicated folders.

### Layout

Application-wide UI elements exposed to consumers:

- `Header`
- `NavigationMenu`
- `Footer`
- `Banner`

### Pages

Business-oriented pages exposed through the framework:

- `ExpertRulePage`
- `GeneratePublicationPage`
- `UserManagementPage`

### Workflows

High-level business actions and scenarios:

- `PublicationWorkflow`
- `AuthenticationWorkflow`

## Architecture Rule

As a general guideline:

> If a consumer test project needs to reference a type directly, that type should not be placed in `Core.Components`.

Types in this folder are implementation details of the framework and may change without affecting consumer code.

## Visibility

Components in this folder should generally be declared as:

```csharp
internal
```

This helps enforce encapsulation and prevents consumers from depending on framework internals.

## Example

### Consumer test

```csharp
await NavigationMenu.GoToAsync(
    InfectionTrackerMenu.Settings.Expertise.ExpertRules);

await Header.LogoutAsync();
```

### Framework internals

```csharp
internal class SpinnerComponent
{
    public Task WaitUntilCompletedAsync();
}

internal class DialogComponent
{
    public Task ConfirmAsync();
}
```

Consumer tests interact with the public API, while the framework relies on internal components to implement the required behavior.