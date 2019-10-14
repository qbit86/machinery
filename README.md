# Machinery

[![Machinery version](https://img.shields.io/nuget/v/Machinery.svg)](https://www.nuget.org/packages/Machinery/)

Minimalistic state machine.

## Basic usage

```cs
MyContext context = …;
IState<MyContext, MyEvent> initialState = new MyState(…);
var stateMachine = new StateMachine<MyContext, MyEvent>(context, initialState);

MyEvent ev = …;
stateMachine.TryProcessEvent(ev);
IState<MyContext, MyEvent> currentState = stateMachine.CurrentState;
```

To use two-parameters generic `StateMachine<C, E>` your states need to implement `IState<C, E>` and optionally `IDisposable`: 

```cs
public interface IState<TContext, TEvent>
{
    bool TryCreateNewState(TContext context, TEvent ev, out IState<TContext, TEvent> newState);
    void OnExiting(TContext context, TEvent ev, IState<TContext, TEvent> newState);
    void OnRemain(TContext context, TEvent ev, IState<TContext, TEvent> currentState);
    void OnEntered(TContext context, TEvent ev, IState<TContext, TEvent> oldState);
}
```

## Advanced usage

```cs
MyContext context = …;
MyState initialState = …;
MyPolicy policy = …;
StateMachine<MyContext, MyEvent, MyState, MyPolicy> stateMachine =
    StateMachine<MyEvent>.Create(context, initialState, policy);

MyEvent ev = …;
stateMachine.TryProcessEvent(ev);
MyState currentState = stateMachine.CurrentState;
```

To use four-parameters generic `StateMachine<C, E, S, P>` you need to provide `IPolicy<C, E, S>` implementation:

```cs
public interface IPolicy<in TContext, in TEvent, TState>
{
    bool TryCreateNewState(TContext context, TEvent ev, TState currentState, out TState newState);
    void OnExiting(TContext context, TEvent ev, TState currentState, TState newState);
    void OnRemain(TContext context, TEvent ev, TState currentState);
    void OnEntered(TContext context, TEvent ev, TState currentState, TState oldState);
    void DisposeState(TContext context, TEvent ev, TState stateToDispose);
}
```

In this case your states are not required to implement any interfaces (may be enums, strings, or whatever).
But you need then to dispatch the runtime type of `currentState` by yourself.
