# DonationHub — Sprint 2: Topics, CQRS, Result Pattern & Unit Testing

This `README.md` documents the **8 core tasks and topics** completed during **Sprint 2** in **DonationHub**, covering what was implemented across **CQRS**, the **Result Pattern**, domain entities, FluentValidation validators, MediatR pipeline behaviors, command handlers, and test suite stabilization.

---

## Core Architectural Topics

### 1. CQRS (Command Query Responsibility Segregation) with MediatR
* **Command Isolation:** Every write operation is modeled as a dedicated `record` (such as `CreateCampaignCommand`, `UpdateCampaignCommand`, `CreateCategoryCommand`, `UpdateCategoryCommand`, `LoginCommand`, and `RegisterCommand`) implementing `IRequest<Result>` or `IRequest<Result<TResponse>>`.
* **Dedicated Command Handlers:** Each command is handled by its own `IRequestHandler<TCommand, TResponse>` class responsible for orchestrating application logic—fetching entities via `IGenericRepository<T>`, enforcing domain rules (`Campaign.Create`, `Campaign.Update`), applying concurrency controls (`ICampaignConcurrencyService`), and persisting changes via `IUnitOfWork`.
* **Pipeline Validation (`ValidationBehavior`):** `ValidationBehavior<TRequest, TResponse>` (`IPipelineBehavior`) runs all registered `IValidator<TRequest>` validators before invoking the handler. When validation fails, it short-circuits execution so `next(...)` and the underlying handler are never called.
* **Layer-by-Layer Testing:** Validators, pipeline behavior, and command handlers were tested independently using **xUnit**, **Moq**, and **FluentAssertions**.

### 2. Result Pattern (`Result` & `Result<TValue>`)
* **Explicit Outcomes Over Exceptions for Business/Validation Failures:** Operations return `Result` or `Result<TValue>` exposing `IsSuccess`, `IsFailure`, `Error` (`Code`, `Description`, `ErrorType`), and `ValidationErrors` (`Dictionary<string, string[]>`), rather than relying on exceptions for expected domain, validation, or concurrency failures.
* **Safe Value Access & Conversions:** `Result<TValue>` inherits from `Result`, guards `Value` access when failed, and supports implicit conversions from `TValue` or `Error` for clean handler returns.
* **Pipeline & Handler Alignment:** `ValidationBehavior` constructs a validation `Error` (`ErrorType.Validation`) and passes field-level errors into `Result.Failure(error, errors)` for non-generic `Result` and `Result.Failure<TValue>(error, errors)` for generic `Result<TValue>`, while handlers return domain-specific errors (such as `NotFound` or `ConcurrencyConflict`) without calling `UnitOfWork` when invalid or missing.

---
## Detailed Breakdown of the 8 Tasks

### `Campaign` & `Category` Domain Tests
* **What we did:** Wrote unit tests for `Campaign` and `Category` domain entities to verify business invariants and state rules directly on the domain models.
* **Details:** Tested `Campaign` rules such as rejecting zero/negative goal amounts, preventing goal amounts lower than `RaisedAmount`, blocking updates on `Completed` or `Expired` campaigns, rejecting past end dates, and transitioning campaign status to `Completed` when donations reach the goal; tested `Category` rules including required/max-length names, `CanBeDeleted()` behavior, and update validation.

### Campaign Command Validators Tests (`CreateCampaignCommandValidator` & `UpdateCampaignCommandValidator`)
* **What we did:** Added unit tests covering validation rules for creating and updating campaigns.
* **Details:** Verified required Arabic/English titles and descriptions, valid goal amounts and currencies, future end dates, valid `CategoryId`, and concurrency/row-version validation cases.

### Category Command Validators Tests (`CreateCategoryCommandValidator` & `UpdateCategoryCommandValidator`)
* **What we did:** Added unit tests for category creation and update validators.
* **Details:** Tested empty/whitespace names, name length limits, description rules, and required identifier checks during updates.

### Authentication Command Validators Tests (`LoginCommandValidator` & `RegisterCommandValidator`)
* **What we did:** Added unit tests for authentication command validators.
* **Details:** Tested invalid/missing email or credential fields and edge-case payloads to ensure invalid authentication requests fail validation early.

### MediatR `ValidationBehavior` Pipeline Tests
* **What we did:** Implemented unit tests for `ValidationBehavior<TRequest, TResponse>` to verify how validation errors are aggregated and returned in the MediatR pipeline.
* **Details:** Tested three scenarios: when validation fails (returns failure response with validation errors and never calls `next`), when validation succeeds (calls `next` and returns the handler response), and when no validators are registered (passes straight to `next`).

### Campaign Command Handlers Tests (`CreateCampaignCommandHandler` & `UpdateCampaignCommandHandler`)
* **What we did:** Wrote handler unit tests using `Moq` and `FluentAssertions` for `IGenericRepository<Campaign>`, `IGenericRepository<Category>`, `IUnitOfWork`, `TimeProvider`, and `ICampaignConcurrencyService`.
* **Details:** Covered missing campaign/category cases (`NotFound`), successful campaign creation and updates, verifying repository calls and `UnitOfWork` completion, and returning `CampaignErrors.ConcurrencyConflict` when `DbUpdateConcurrencyException` is thrown.

### Category Command Handlers Tests (`CreateCategoryCommandHandler` & `UpdateCategoryCommandHandler`)
* **What we did:** Wrote handler unit tests for creating and updating categories with mocked repository, `IUnitOfWork`, `TimeProvider`, and mapper dependencies.
* **Details:** Verified successful category creation and updates, `NotFound` responses when updating a non-existent category, and domain exception handling on invalid data without saving changes.

### Task 8: `LoginCommandHandler` Tests, Bug Fixes & Full `dotnet test` Run
* **What we did:** Added unit tests for `LoginCommandHandler`, fixed bugs surfaced during test runs, and executed the test suite (`dotnet test`).
* **Details:** Fixed `ValidationBehavior.CreateValidationFailureResponse` to resolve and invoke `Result.Failure<TValue>(error, validationErrors)` properly for generic `Result<T>` responses, fixed `UpdateCampaignCommandHandler` so `request.EndDate` (instead of `time`) is passed to `campaign.Update(...)`, and verified all **61** tests pass (`total: 61, failed: 0`)[cite: 6].

---

## Running Tests

```bash
dotnet test .\DonationHub.UnitTests\DonationHub.UnitTests.csproj --verbosity normal
