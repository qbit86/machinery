# Product Overview

Machinery is a minimalistic .NET library for implementing state machines.
It provides a clean, type-safe API for building deterministic finite automata (DFA) with both synchronous and asynchronous support.

## Core Concepts

- **State Machine**: Central abstraction for managing state transitions
- **IState Interface**: Defines state behavior with lifecycle methods (OnEntering, OnEntered, OnExiting, OnExited, OnRemain)
- **Context & Events**: Generic parameters allowing flexible state machine implementations
- **Async Support**: Full async/await support through IAsyncState and AsyncStateMachine

## Key Features

- Type-safe state transitions
- Lifecycle event hooks for state changes
- Support for both value and reference type states
- Polymorphic state implementations
- Multi-target framework support (.NET Framework 4.6.1, .NET Standard 2.0/2.1)
- Minimal dependencies and lightweight design

## Target Audience

Developers building applications that require state management, workflow engines, game state systems, or any scenario where explicit state transitions need to be modeled and controlled.
