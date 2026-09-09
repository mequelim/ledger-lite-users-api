# How to use the custom exceptions that were created?

# Introduction

This document describes the pattern to follow for domain exceptions in ***any*** microservice of `Ledger Lite` (`UsersAPI`, `AccountsAPI`, `TransactionsAPI`, `BudgetAPI`, `DashboardAPI`, etc.).

The reference example used in this document is the `User` domain (project `LedgerLite.Users`), but the convention applies **equally** to any entity in any service.

# Folder Structure

- Each microservice maintains its own domain exceptions, grouped by ***entity***, inside the `Domain` project:

    ```markdown
    {Service}.Domain.Errors
    ├── DomainException.cs
    └── {Entity}Exceptions/
        ├── {Entity}NotFoundException.cs
        ├── {Entity}AlreadyExistsException.cs
        └── ...
    ```


## Example in `UsersAPI`

```markdown
LedgeLiteUsers.Domain.Errors
├── BankAccountExceptions/
    ├── BankAccountAlreadyActiveException.cs
    ├── BankAccountAlreadyExistsException.cs
    ├── BankAccountAlreadyInactiveException.cs
    ├── BankAccountNotFoundException.cs
    ├── InvalidBankAccountAccountNumberException.cs
    └── InvalidBankAccountAgencyException.cs
├── UserExceptions/
    ├── UserNotFoundException.cs
    ├── UserAlreadyExistsException.cs
    ├── DuplicateEmailException.cs
    ├── InvalidUserEmailException.cs
    ├── InvalidUserPhoneException.cs
    ├── UserInactiveException.cs
    ├── UserAlreadyInactiveException.cs
    └── UserAlreadyActiveException.cs
└── DomainException.cs
```

## `DomainException` — the base class

- Every domain exception, in any service, inherits from an abstract base class:

    ```csharp
    namespace {Service}.Domain.Errors
    {
        public abstract class DomainException : Exception
        {
            protected DomainException(string message) : base(message) { }

            protected DomainException(string message, Exception innerException)
		        : base(message, innerException) { }
         }
    }
    ```


### Why?

- Allows catching/logging any domain error generically (`isDomainException`), without listing every type;
- Allows specializing the handling (status code, payload) only where necessary;
- Clearly separates domain errors (business rule violated) from infrastructure errors (`DbUpdateException`, `TimeoutException`, etc.), which should not be handled the same way.

> [!NOTE]
>
> Since all `Ledger Lite` microservices will follow this same pattern, evaluate whether `DomainException` should become a shared package/project (e.g. `LedgerLite.SharedKernel`) instead of being duplicated in each service. If each service evolves the class independently (e.g. one service adds an error field that another doesn't need), keeping it local to each one may be preferable.

# Conventions for Every Domain Exception

- Regardless of the service or entity:
    - Inherits from `DomainException`;
    - The `DomainException` class is sealed (`sealed`), since domain exceptions are not meant for further inheritance;
    - Exposes the error's identifying data as a **property** (`{ get; }`) — never a public field (e.g. `UserId`, `AccountId`, `Email`, `TransactionId`);
    - One file per class;
    - Follows the pattern of 3 (three) constructors:
        1. **With the key(s)**: automatically generates a default message;
        2. **With the key(s) + custom message**;
        3. **With the key(s) + custom message + inner exception**.

# Generic Template

```csharp
namespace {Service}.Domain.Errors.{Entity}Exceptions
{
    public sealed class {Entity}{ErrorType}Exception : DomainException
    {
        public {KeyType} {KeyName} { get; }

        // Constructor with {key}:
        public {Entity}{ErrorType}Exception({KeyType} {key})
            : base($"...") => {KeyName} = {key};

        // Constructor with {key} and custom message:
        public {Entity}{ErrorType}Exception({KeyType} {key}, string message)
		        : base(message) => {KeyName} = {key};

        // Constructor with {key}, custom message and inner exception:
        public {Entity}{ErrorType}Exception({KeyType} {key}, string message, Exception innerException)
            : base(message, innerException) => {KeyName} = {key};
    }
}
```

## Common Exception Categories per Entity

- When modeling exceptions for a new entity, we should consider the same categories used for `User`:
    - **Not found**: `{Entity}NotFoundException`;
    - **Conflict/duplicity**: `{Entity}AlreadyExistsException`, `Duplicate{Field}Exception`;
    - **Invalid data validation**: `Invalid{Entity}{Field}Exception`;
    - **State/business rule**: `{Entity}InactiveException`, `{Entity}Already{State}Exception`;
    - **Relationship**: `{Entity}{Relationship}NotFoundException`, `{Entity}Already{Has}{Relationship}Exception`.

## Reference example: `User` domain (UsersAPI)

### Exception catalog

| **Exception** | **Key** | **Scenario** | **Suggested HTTP Status** |
| --- | --- | --- | --- |
| `UserNotFoundException` | `UserId` | User not found by Id | `404` |
| `UserAlreadyExistsException` | `UserId` | User already exists | `409` |
| `DuplicateEmailException` | `Email` | Email already in use by another user | `409` |
| `InvalidUserEmailException` | `Email` | Email in invalid format | `400` |
| `InvalidUserPhoneException` | `Phone` | Phone in invalid format (current validation is BR-only) | `400` |
| `UserInactiveException` | `UserId` | Operation not allowed — user inactive | `409` |
| `UserAlreadyInactiveException` | `UserId` | Attempt to deactivate an already inactive user | `409` |
| `UserAlreadyActiveException` | `UserId` | Attempt to activate an already active user | `409` |

# Usage Pattern

<aside>
ℹ️

Pattern valid for ***any*** service/entity.

</aside>

## Usage options

### In the repository/handler that fetches the entity

```csharp
public class {Entity}Repository : I{Entity}Repository
{
    public async Task<{Entity}> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.{Entities}
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return entity ?? throw new {Entity}NotFoundException(id);
    }
}
```

### Inside a handler/use case (Vertical Slice)

```csharp
public class Get{Entity}ByIdHandler
{
    private readonly I{Entity}Repository _repository;

    public Get{Entity}ByIdHandler(I{Entity}Repository repository) => _repository = repository;

    public async Task<{Entity}Response> Handle(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new {Entity}NotFoundException(
								id,
		            $"Could not resolve {nameof({Entity})} '{id}' for this operation."
		        );

        return {Entity}Response.FromEntity(entity);
    }
}
```

## Handling at the API edge (WebAPI)

- A single pattern, replicable across any service: each `WebAPI` maps its own service's domain exceptions to HTTP status codes, with a generic fallback via `DomainException` for anything without a specific mapping.

### Option A: middleware with `UseExceptionHandler`

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

        // Specific handling per type (must come before the generic one):
        if(exceptionFeature?.Error is UserNotFoundException notFoundEx)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "UserNotFound",
                userId = notFoundEx.UserId,
                message = notFoundEx.Message
            });

            return;
        }

        // ... other service-specific types ...

        // Generic fallback for any domain error not explicitly mapped:
        if(exceptionFeature?.Error is DomainException domainEx)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new
            {
                error = domainEx.GetType().Name,
                message = domainEx.Message
            });

            return;
        }

        // Infrastructure / unmapped errors: generic 500.
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    });
});
```

### Option B: `IExceptionHandler` (recommended, .NET 8+)

- More aligned with Clean Architecture/Vertical Slice: one handler per type (or group), registered via DI, without a giant `if/else`. Each service only registers the handlers relevant to its domain.

```csharp
public sealed class UserNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken
    )
    {
        if(exception is not UserNotFoundException ex) return false;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            error = "UserNotFound",
            userId = ex.UserId,
            message = ex.Message
        }, cancellationToken);

        return true;
    }
}
```

```csharp
builder.Services.AddExceptionHandler<UserNotFoundExceptionHandler>();
// one AddExceptionHandler<T> per exception type/group in the service
builder.Services.AddProblemDetails();
// ...
app.UseExceptionHandler();
```