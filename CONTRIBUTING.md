# Contributing Guidelines

Thank you for your interest in contributing to **User API**!
This document outlines the rules, expectations, and workflow to ensure a clean, consistent, and high-quality codebase.

---

## 📌 General Principles

+ All contributions **must be made through Pull Requests (PRs)**;
+ All PRs must target the correct base branch:
  + `development`: for new features, improvements and bug fixes.
  + `homol`: for release preparation and staging.
  + `master`: → **do NOT commit directly**. Only CI/CD deploy merges are allowed.
+ Code must follow the project's **coding standards**, **commit conventions**, and **branching strategy**;
+ Every PR must pass:
  + Automated build;
  + Automated tests;
  + Static analysis (if enabled);
  + Code review approval.

---

## 🧱 Repository Structure

The project follows **Vertical Slice Architecture** and **Clean Architecture** with strict separation of concerns:

+ `Controllers/`;
+ `Slices/`;
+ `Domain/`;
+ `Infrastructure/`;
+ `Shared/`;
+ `tests/`.

Please keep new code inside the correct folder and respect existing patterns.

---

## 🚀 How to Contribute

### 1. Fork and Clone the Repository

```shell
git clone git@github.com:mequelim/on-garage-employee-api.git
cd on-garage-employee-api
```

---

### 2. Create a New Branch

+ Follow the naming rules in [`BRANCH_STRATEGY.md`](./docs/BRANCH_STRATEGY.md):

  ```shell
  git checkout -b <type>/OGG-{task-number}-{task-description}: <short desc
  ```

#### Examples

```shell
feature/LL-123-user-services
fix/LL-158-user-services
chore/LL-200-update-ci-workflow
```

---

### 3. Follow Commit Conventions

+ Every commit must follow Conventional Commits as described in [`COMMIT_MESSAGE_CONVENTIONS.md`](./docs/COMMIT_MESSAGE_CONVENTIONS.md).

#### Examples

```shell
feature/LL-{task-number}-{task-description}: {short-description}
fix/LL-{task-number}-{task-description}: {short-description}
test/LL-{task-number}-{task-description}: {short-description}
```

---

### 4. Write and Run Tests

+ This project uses TDD guidelines:
  + Unit tests for domain logic;
  + Tests must pass before submitting your PR;
  + Run tests locally:

    ```shell
    dotnet test
    ```

---

### 5. Submit a Pull Request

+ Open a PR to the appropriate branch:
  + `development` for ongoing work;
  + `homol` for release preparation.
+ Add a clear and concise description;
+ Link related issues;
+ Ensure all CI checks pass;
+ Request one reviewer minimum.

---

## 🔍 Code Review Guidelines

+ Reviewers will check for:
  1. Correct branch target;
  2. Proper commit messages;
  3. Code style and naming consistency;
  4. Alignment with architecture;
  5. Tests covering new or modified behavior;
  6. No unnecessary changes (formatting noise, unused imports, etc.).

---

## 🙌 Thank You!

Your contribution helps make Garage API a solid, scalable, and maintainable system.
