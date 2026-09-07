# 👨‍🔧 LedgerLiteUsersAPI.UsersAPI

**.NET 10 • C# 14 • Vertical Slice Architecture • Clean Architecture • PostgreSQL • Redis • Docker • GitHub Actions CI/CD**

---

## Overview

`LedgerLiteUsersAPI` is a **backend service** designed to **manage users of, automotive mechanics**, following principles of **Coherence Modelling**, the **Vertical Slice Architecture** and the **Clean Architecture**.

This microsservice is intentionally modular, scalable and prepared for future evolution into independent bounded contexts or microservices.

---

## 📑 Table of Contents

+ [Architecture Overview](#architecture-overview)
+ [Project Structure](#project-structure)
+ [Development Environment](#development-environment)
+ [Running with Docker](#running-with-docker)
+ [Migrations](#migrations)
+ [Vertical Slice Structure](#vertical-slice-structure)
+ [Context Map](#context-map)
+ [API Documentation](#api-documentation)
+ [Testing](#testing)
+ [CI/CD Pipeline](#cicd-pipeline)
+ [Conventions & Coding Standards](#conventions--coding-standards)

---

## 🧱 Architecture Overview

+ The `LedgerLiteUsersAPI` project follows:

  + **Vertical Slice Architecture**;

    + Each feature owns its commands, handlers, validators, and mappings.

  + **Coherence Modelling**

    + Entities, Value Objects, Aggregates, and business rules are implemented cleanly and independently.

  + **REST API with Controllers (no Minimal APIs)**

    + Chosen for clearer separation, testability, and maintainability.

  + **PostgreSQL + EF Core**

    + snake_case naming convention, clean database modeling, and migrations included.

  + **Dockerized Development Environment**

    + Seamless startup with Docker Compose.

### Solution Structure

> [!IMPORTANT]
>
> Update this topic at the end of the project.

```
LedgerLiteUsersAPI/
├── LedgerLiteUsers.slnx
└── src/
    ├── LedgerLiteUsers.Domain/           # Core domain (no dependencies)
    │   ├── Abstractions/
    │   │   ├── Result.cs                            # Success/Failure result wrapper
    │   │   └── Errors/
    │   │       ├── Error.cs                         # Error type definition
    │   │       └── ValidationError.cs               # Validation error collection
    │   └── Entities/
    │       ├── AuditableEntity.cs                   # Base class with CreatedOn/UpdatedOn
    │       └── Book.cs                              # Domain entity
    │
    ├── LedgerLiteUsers.Application/      # Use cases & vertical slices
    │   ├── DependencyInjection.cs                   # Application layer DI registration
    │   ├── GlobalUsings.cs                          # ASP.NET Core global usings
    │   ├── Abstractions/
    │   │   ├── IApiEndpoint.cs                      # Endpoint contract
    │   │   ├── IHandler.cs                          # Handler contract
    │   │   └── Data/
    │   │       ├── IRepository.cs                   # Repository abstraction
    │   │       └── IUnitOfWork.cs                   # Unit of Work abstraction
    │   ├── Constants/
    │   │   └── ApiTags.cs                           # OpenAPI tags
    │   ├── Extensions/
    │   │   ├── MapEndpointExtensions.cs             # Endpoint registration
    │   │   └── ResultExtensions.cs                  # Result pattern matching
    │   ├── Features/                                # Vertical slices (features)
    │   │   └── BookFeature/
    │   │       ├── BookErrors.cs                    # Feature-specific errors
    │   │       ├── CreateBook/
    │   │       │   ├── CreateBookHandler.cs         # Handler + Request/Response records
    │   │       │   ├── CreateBookValidator.cs
    │   │       │   └── CreateBookEndpoint.cs
    │   │       ├── GetAllBooks/
    │   │       │   ├── GetAllBooksHandler.cs
    │   │       │   └── GetAllBooksEndpoint.cs
    │   │       ├── GetBookById/
    │   │       │   ├── GetBookByIdHandler.cs
    │   │       │   ├── GetBookByIdValidator.cs
    │   │       │   └── GetBookByIdEndpoint.cs
    │   │       ├── UpdateBook/
    │   │       │   ├── UpdateBookHandler.cs
    │   │       │   ├── UpdateBookValidator.cs
    │   │       │   └── UpdateBookEndpoint.cs
    │   │       └── DeleteBook/
    │   │           ├── DeleteBookHandler.cs
    │   │           ├── DeleteBookValidator.cs
    │   │           └── DeleteBookEndpoint.cs
    │   └── Pipelines/                               # Request processing decorators
    │       ├── ValidationDecorator.cs
    │       └── LoggingDecorator.cs
    │
    ├── LedgerLiteUsers.Persistence/   # External concerns                  # Infrastructure DI registration
    │   ├── Database/
    │   │   └── ApplicationDbContext.cs              # EF Core DbContext
    │   ├── Interceptors/
    │   │   └── AuditInterceptor.cs                  # Auto CreatedOn/UpdatedOn
    │   ├── Migrations/
    │   │   └── ...
    │   └── Repository/
    │       ├── Repository.cs                        # Generic repository implementation
    │       └── UnitOfWork.cs                        # Unit of Work implementation
    │
    └── LedgerLiteUsers.WebApi/           # Thin host / entry point
        ├── DependencyInjection.cs
        ├── Program.cs                               # App startup & DI composition
        ├── appsettings.json
        ├── appsettings.Development.json
        ├── Exceptions/
        │   └── CustomExceptionHandler.cs            # Global exception handler
        └── Extensions/
            └── HealthChecksExtensions.cs            # Health check configuration
```

### Clean Architecture Layers

> [!WARNING]
>
> Update this topic at the end of the project.

```markdown
┌─────────────────────────────────────────────┐
│                  WebApi                      │  ← Entry point, thin host
│         (Program.cs, Exception Handler)     │
├─────────────────────────────────────────────┤
│              Infrastructure                  │  ← EF Core, Repository, Interceptors
│      (External World)    │
├─────────────────────────────────────────────┤
├─────────────────────────────────────────────┤
│              Persisttence                  │  ← EF Core, Repository, Interceptors
│      (DbContext, Repository, UnitOfWork)    │
├─────────────────────────────────────────────┤
│               Application                    │  ← Vertical slices live here
│  (Features, Handlers, Validators, Endpoints)│
├─────────────────────────────────────────────┤
│                  Domain                      │  ← Entities, Result, Errors
│      (Book, AuditableEntity, Error)         │
└─────────────────────────────────────────────┘
```

### Complete Architecture (C4 - Container) Diagram

![Complete Architecture (C4 - Container) Diagram](./docs/diagrams/[Ledger%20Lite]%20Arquitetura%20Completa%20(C4%20-%20Container).jpg)

---

## 📂 Project Structure

> [!WARNING]
>
> Add the project structure.

---

## 🔧 Environment Setup

+ This project uses a single `.env` file at the repository root.
+ However, this file is not committed — instead, it is generated locally using a helper script:

### Generate `.env` automatically

+ The script creates:

  + `.env` with default values;
  + Leaves `.env.example` untouched;
  + Avoids committing sensitive data.

#### macOS / Linux (Ubuntu) [Bash]

```shell
bash scripts/generate-env.sh
```

#### Windows 11 (PowerShell) [with WSL]

```shell
wsl bash scripts/generate-env.sh
```

#### Windows 11 (PowerShell) [without WSL]

> [!INFO]
>
> You need to use **Git Bash**!

```shell
./scripts/generate-env.sh

# Or (use PowerShell)

& "C:/Program Files/Git/bin/bash.exe" ./scripts/generate-env.sh
```

---

## 🛠 Development Environment

### Requirements

+ .NET 10 SDK;
+ Docker & Docker Compose;
+ PostgreSQL (if running outside Docker);
+ GitHub account (for CI/CD).

### Build project

```shell
dotnet build --configuration Release
```

### Restore dependencies

```shell
dotnet restore
```

---

## 🐳 Running with Docker and `.env`

### To start API + PostgreSQL

```shell
docker compose up --build
```

### To stop containers

```shell
docker compose down
```

### API will run on

```shell
http://localhost:8080
```

### PostgreSQL runs on

```shell
localhost:5432
user: garage
password: garage123
database: garage
```

---

## 🗃 Migrations

### Update Entity Framework Core (EF Core)

```shell
dotnet tool update --global dotnet-ef
```

### To add migration

```shell
# cd LedgerLiteUsers.Persistence\
dotnet ef migrations add {MigrationName} --project .\LedgerLiteUsers.Infrastructure\ --startup-project .\LedgerLiteUsers.WebApi\ --output-dir Persistence\Migrations
```

> [!NOTE]
>
> Change the `{MigrationName}` field to the name you want to give your migration!

### To apply migration

```shell
# cd LedgerLiteUsers.Persistence\
dotnet ef database update --project .\LedgerLiteUsers.Infrastructure\ --startup-project .\LedgerLiteUsers.WebApi\
```

### To remove a migration

```shell
# cd LedgerLiteUsers.Persistence\
dotnet ef migrations remove --project .\LedgerLiteUsers.Infrastructure\ --startup-project .\LedgerLiteUsers.WebApi\ --output-dir Persistence\Migrations
```

---

## 🧩 Vertical Slice Structure

> [!NOTE]
>
> This structure isolates business logic and promotes scalability.

+ Each slice contains:

```markdown
LedgerLiteUsersAPI.Users
├── Update/
    ├── Command.cs
    ├── Handler.cs
    ├── Validator.cs
    ├── Response.cs
    └── MappingProfile.cs
```

### Example

```markdown
Feature/User/CreateUser/
  CreateUserCommand.cs
  CreateUserHandler.cs
  CreateUserValidator.cs
  CreateUserResponse.cs
```

---

## 🗺 Context Map (simplified)

![Operation Diagram](./docs/diagrams/[Ledger%20Lite]%20Complete%20Architecture%20(C4%20-%20Container).jpg)

> [!NOTE]
>
> Each context communicates only through well-defined application boundaries.

### How are `LedgerLiteUsers` endpoints prepared to communicate with the `Transaction` service?

![Endpoints's Structure](./docs/diagrams/[Ledger%20Lite]%20UsersAPI.jpg)

---

## 📘 API Documentation

> [!WARNING]
>
> Update link at the end of the project!

+ Scalar auto-generates documentation at `{link}`.

---

## 🧪 Testing

+ `LedgerLiteUsersAPI` uses:
  + xUnit;
  + FluentAssertions;
  + TDD methodology;
  + Tests isolated by Vertical Slice.

### Run all tests

```shell
dotnet test
```

---

## 🔐 Required GitHub Secrets

| Secret Name                    | Description                                 |
| ------------------------------ | ------------------------------------------- |
| `POSTGRES_PASSWORD`            | Password for PostgreSQL in all environments |
| `POSTGRES_USER`                | Username for PostgreSQL                     |
| `POSTGRES_DB`                  | Database name                               |
| `PRODUCTION_CONNECTION_STRING` | Production DB connection string             |
| `STAGING_CONNECTION_STRING`    | Staging DB connection string                |

---

## 🚀 CI/CD Pipeline

### CI/CD Environment Injection

+ During the CI/CD pipeline:

  + `.env` is generated dynamically;
  + Secrets are pulled from GitHub;
  + The Docker image is built using injected values;
  + No environment file is stored in the repository;
  + This ensures security and flexibility across environments.

### CI

+ Restore, build, tests, coverage;
+ Runs on every push.

### CD

+ Docker image built & pushed to GHCR;
+ Deployments triggered per branch:
  + `development` → DEV environment;
  + `homol` → Homologation environment;
  + `master` → Production

### Approvals Required

+ **Production** and **homologation** deploys require manual approval via **GitHub Environments**.

---

## ✨ Conventions & Coding Standards

+ C# 14 features enabled;
+ Nullable reference types active;
+ EF Core uses snake_case naming;
+ Controllers are thin;
+ Application logic lives in slices;
+ Entities contain domain rules;
+ Services avoided unless absolutely necessary;
+ Async everywhere (async/await default);
+ Clean exception handling;
+ Clear separation of concerns.

---

## 📄 License

+ MIT License;
+ Free to use, modify and distribute.

---

## 👨‍💻 Author

+ Pedro Henrique Mequelim da Silva;
+ Mobile and FullStack .NET Developer & Software Architecture;
+ Brazil, 🇧🇷
