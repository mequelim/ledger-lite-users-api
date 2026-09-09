# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
+

### Changed
+

### Deprecated
+

### Removed
+

### Fixed
+

### Security
+

---

## [1.1.0] - 2026-07-15

### Added
+ One-click reorder for the last 5 orders per coffee shop (#144)
+ PIX payment support in the digital wallet (#138)
+ Push notification upon reaching 80% of the order limit on the Basic plan (#135)

### Changed
+ Expiration time for contextual coupons reduced from 48h to 24h (#140)
+ Wallet balance limit increased from R$300.00 to R$500.00 (#137)

### Fixed
+ Fixed point calculation when an order contains an item with an active discount (#143)
+ Virtual queue displaying incorrect position for simultaneous orders (#139)

---

## [1.0.1] - 2026-06-28

### Fixed
+ Geolocation alert triggering outside the configured radius on Android devices (#131)
+ Crash when attempting to access the specialty coffee map without an internet connection (#129)
+ Stamps not correctly reversed after order cancellation within 2 minutes (#127)

### Security
+ Updated `jsonwebtoken` dependency to fix a security vulnerability (CVE-2026-XXXX) (#133)

---

## [1.0.0] - 2026-06-01

### Added
+ Initial launch of Blendis
+ Advance ordering (mobile order & pay) with push notification confirmation
+ Smart geolocation with alerts sent to the barista when the user is 5 minutes away
+ Digital wallet supporting credit cards, debit cards, and PIX
+ Loyalty program featuring points, stamps, and cashback
+ Coffee Club — monthly subscription including daily or weekly coffee
+ Automated contextual coupons (time of day, weather, events)
+ Real-time virtual queue
+ Ratings for coffee shops and blends
+ Suspended Coffee (Pay it Forward)
+ "My Full Cup" discount
+ End-of-day item sales with a minimum 30% discount
+ Smart push notifications

---

## Types of changes

| Type | Description |
|---|---|
| `Added` | New features |
| `Changed` | Changes to existing features |
| `Deprecated` | Features to be removed in future versions |
| `Removed` | Removed features |
| `Fixed` | Bug fixes |
| `Security` | Security vulnerability fixes |

---

## Versioning

This project follows [Semantic Versioning](https://semver.org/):

+ **MAJOR** (`x.0.0`) — breaking changes (incompatible with previous versions)
+ **MINOR** (`0.x.0`) — new backward-compatible features
+ **PATCH** (`0.0.x`) — backward-compatible bug fixes

---

[Unreleased]: https://github.com/qoherent/blendis/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/qoherent/blendis/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/qoherent/blendis/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/qoherent/blendis/releases/tag/v1.0.0