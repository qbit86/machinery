# Machinery

Minimalistic state machine.

## Basic usage

```cs
var context = new MyContext(…);
IState<MyContext, MyEvent> initialState = new MyState(…);
var stateMachine = new StateMachine<MyContext, MyEvent>(context, initialState);

var ev = new MyEvent(…);
stateMachine.TryProcessEvent(ev);
IState<MyContext, MyEvent> currentState = stateMachine.CurrentState;
```

## Advanced usage

```cs
var context = new MyContext(…);
var initialState = new MyState(…);
var policy = new MyPolicy(…);
StateMachine<MyContext, MyEvent, MyState, MyPolicy> stateMachine =
    StateMachine<MyEvent>.Create(context, initialState, policy);

var ev = new MyEvent(…);
stateMachine.TryProcessEvent(ev);
MyState currentState = stateMachine.CurrentState;
```
