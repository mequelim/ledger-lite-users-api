# Branch Strategy

This project uses a **lightweight GitFlow-inspired strategy** optimized for CI/CD automation.

There are **three primary branches**:

---

## 🌱 `development` (Main working branch)

+ All **new features** and **fixes** must branch from here;
+ Pull requests should target this branch;
+ CI builds run on every push and PR.

---

## 🧪 `homol` (Staging / Pre-production)

+ Used for **approval**, **UAT**, and **staging deployments**;
+ Only merged from `development`;
+ Represents a **"release candidate" state**;
+ Requires **review** + **approval** before merging.

---

## 🚀 `master` (Production)

+ Represents the **production-ready code**;
+ Only updated through **approved merges from `homol`;
+ Protected against direct pushes.

> [!INFO]
>
> CI/CD deploys to production when code is merged into this branch.

---

## Branch naming rules

+ Branches must follow:

  ```shell
  <type>/SOAT-{task-number}-{task-description}: <short-description>
  ```

### Allowed types

+ `feature/SOAT-{task-number}-{task-name}`: new features;
+ `fix/SOAT-{task-number}-{task-name}`: bug fixes;
+ `hotfix/SOAT-{task-number}-{task-name}`: production fixes;
+ `chore/SOAT-{task-number}-{task-name}`: tooling, configs, dependencies;
+ `refactor/SOAT-{task-number}-{task-name}` — internal improvements;
+ `test/SOAT-{task-number}-{task-name}` — test-related changes;
+ `docs/SOAT-{task-number}-{task-name}` — documentation.

### Examples:

```shell
feature/{task}-create-service-order
fix/{task}-stock-movement-bug
refactor/{task}-os-validation
docs/{task}-update-readme
chore/{task}-github-actions-secrets
hotfix/{task}-payment-timeout
```

---

## Pull Request Rules

+ PRs should be **small and focused**;
+ Must follow commit conventions;
+ Must reference related issue (if applicable);
+ Must pass all CI checks;
+ Must be reviewed by at least one maintainer.

---

## Release Flow Summary

```markdown
development → homol → master
```

+ Developers commit and PR into `development`;
+ Once stable → merge into `homol` (staging);
+ Once approved → merge into `master` (production);
+ CI/CD automatically deploys on `homol` and `master`.

---

## Hotfixes

+ Urgent production issues follow this rule:

  1. Branch from `master`
  2. Name: `hotfix/<description>`
  3. After fix:
    + Merge into `master`;
	  + Merge into `homol`;
	  + Merge into `development`.

> [!INFO]
>
> This ensures history stays consistent across all branches.

---

## Final Notes

+ This strategy ensures:
  + Clean and predictable flow;
  + Safe production releases;
  + Isolated development per feature;
  + Traceable change history;
  + CI/CD-friendly branching.