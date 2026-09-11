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

  + **Coherence Modelling**;

    + Entities, Value Objects, Aggregates, and business rules are implemented cleanly and independently.

  + **REST API with Controllers (no Minimal APIs)**;

    + Chosen for clearer separation, testability, and maintainability.

  + **PostgreSQL + EF Core**;

    + snake_case naming convention, clean database modeling, and migrations included.

  + **Dockerized Development Environment**.

    + Seamless startup with Docker Compose.

### Solution Structure

> [!IMPORTANT]
>
> Update this topic at the end of the project.

```
LedgerLiteUsersAPI/
├── src/
    ├──Tests/
    │   ├── Users.Domain.Tests/
    ├── Users.Domain/           # Core domain (no dependencies)
    │   ├── Entities/
    │   │   ├── Enums
    |   │   │   └── BankAccountType.cs
    │   │   ├── BankAccount.cs
    │   │   ├── BaseEntity.cs
    │   │   └── User.cs
    │   ├── Exceptions
    │   │   ├── BankAccountExceptions/
    |   │   │   ├── BankAccountAlreadyActiveException.cs
    |   │   │   ├── BankAccountAlreadyExistsException.cs
    |   │   │   ├── BankAccountAlreadyInactiveException.cs
    |   │   │   ├── BankAccountNotFoundException.cs
    |   │   │   ├── InvalidBankAccountAccountNumberException.cs
    |   │   │   └── InvalidBankAccountAgencyException.cs
    │   │   ├── UserExceptions/
    |   │   │   ├── DuplicateEmailException.cs
    |   │   │   ├── InvalidUserAgeException.cs
    |   │   │   ├── InvalidUserEmailException.cs
    |   │   │   ├── InvalidUserPhoneException.cs
    |   │   │   ├── UserAlreadyActiveException.cs
    |   │   │   ├── UserAlreadyExistsException.cs
    |   │   │   ├── UserAlreadyInactiveException.cs
    |   │   │   ├── UserInactiveException.cs
    |   │   │   └── UserNotFoundException.cs
    │   │   └── DomainException.cs
    │   ├── Interfaces
    │   │   ├── Auditable
    |   |   |   └── IAuditableEntity.cs
    │   │   └── Validators
    |   │   │   ├── IBankAccountDataValidator.cs
    |   |   │   └── IUserDataValidator.cs
    │   ├── Validators
    │   │   ├── BankAccountValidator.cs
    │   │   └── UserValidator.cs
    │   ├── .gitignore
    │   └── Users.Domain.csproj
    ├── .gitignore
    └── LedgerLiteUsersAPI.slnx
├── .editorconfig
├── .env.example
├── .gitignore
├── CONTRIBUTING.md
├── LICENSE.md
└── README.md
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

> [!IMPORTANT]
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

> [!IMPORTANT]
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
