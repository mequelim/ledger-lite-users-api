# Commit Message Conventions

This project uses **Conventional Commits** to maintain a clear, structured, and automated-friendly commit history.

---

## Format

```shell
<type>/LL-{task-number}-{task-description}: <short description>
```

---

## 📌 Allowed commit types

| Type       | Meaning |
|------------|---------|
| **feature/**   | New feature added |
| **fix/**    | Bug fix |
| **docs/**   | Documentation changes |
| **style/**  | Code formatting only (no logic changes) |
| **refactor/** | Internal code changes without altering behavior |
| **perf/**   | Performance improvements |
| **test/**   | Adding or updating tests |
| **chore/**  | CI/CD, tooling, dependencies, configs |
| **build/**  | Build system changes |
| **ci/**     | CI pipeline changes |

---

## 🧭 Scope (optional)

### Examples

```shell
feature/ABC-001-create-an-endpoint-to-get-client-by-id: create an endpoint to get client by id
fix/ABC-002-fix-errors-to-get-client-by-id: correct the endpoint to get client by id logic
test/ABC-003-create-tests-for-endpoint-to-get-client-by-id: add unit tests for endpoint to get client by id
```

---

## ✏️ Description rules

+ Use **imperative tone** ("add", "create", "fix", not "added", "fixed");
+ Max 72 characters recommended;
+ Keep lowercase whenever possible;
+ Be direct and explicit.

---

## 🔥 Examples of good commits

```shell
feat(order): implement diagnostics workflow
fix(stock): adjust quantity calculation for parts
refactor(shared): extract base validator
test(os): add tests for order approval use case
docs: update README with docker instructions
chore: upgrade EF Core to latest version
```

---

## 🚫 Bad Commit examples

```bash
update stuff
fix bugs
wip
changes
final version
```

> [!CAUTION]
>
> These will be rejected during review.

---

## 🎯 Why This Matters

+ Cleaner history;
+ Better pull request reviews;
+ Easier automated releases;
+ Better traceability of changes.