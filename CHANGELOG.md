# Changelog

## [0.7.0] - 2021-05-06
### Removed
- ``StateMachine`2``, ``DisposableStateMachine`2``, ``StateMachine`4``, ``DisposableStateMachine`4``

## [0.6.1] - 2021-02-25
### Added
- Public property `Context`.

## [0.6.0] - 2021-01-15
### Added
- Nullability attributes for legacy target frameworks.

## [0.5.0] - 2020-12-13
### Changed
- Annotate API for nullability.

## [0.4.0] - 2019-10-20
### Added
- `IDisposable` constraint for `TState` type parameter of `DisposableStateMachine<…>` generics.

### Removed
- Context constraint to be nonnull.

## [0.3.0] - 2019-10-16
### Added
- `DisposableStateMachine<…>` classes owning their states.

### Removed
- `DisposeState(…)` method.

## [0.2.0] - 2019-10-15
### Removed
- Redundant parameter from `OnRemain(…)` signature.

## [0.1.1] - 2019-10-12
### Added
- [SourceLink](https://github.com/dotnet/sourcelink) support.

### Fixed
- Package metadata.

## [0.1.0] - 2019-10-11
### Added
- Basic building blocks for generic state machines: `StateMachine<…>`, `IState<…>`, `IPolicy<…>`.

[Unreleased]: https://github.com/qbit86/machinery/compare/machinery-0.7.0...HEAD
[0.7.0]: https://github.com/qbit86/machinery/compare/machinery-0.6.1...machinery-0.7.0
[0.6.1]: https://github.com/qbit86/machinery/compare/machinery-0.6.0...machinery-0.6.1
[0.6.0]: https://github.com/qbit86/machinery/compare/machinery-0.5.0...machinery-0.6.0
[0.5.0]: https://github.com/qbit86/machinery/compare/machinery-0.4.0...machinery-0.5.0
[0.4.0]: https://github.com/qbit86/machinery/compare/machinery-0.3.0...machinery-0.4.0
[0.3.0]: https://github.com/qbit86/machinery/compare/machinery-0.2.0...machinery-0.3.0
[0.2.0]: https://github.com/qbit86/machinery/compare/machinery-0.1.1...machinery-0.2.0
[0.1.1]: https://github.com/qbit86/machinery/compare/machinery-0.1.0...machinery-0.1.1
[0.1.0]: https://github.com/qbit86/machinery/releases/tag/machinery-0.1.0
