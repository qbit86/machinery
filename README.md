# Machinery

[![Machinery version](https://img.shields.io/nuget/v/Machinery.svg?logo=nuget)](https://www.nuget.org/packages/Machinery/)

Machinery is a minimalistic .NET library for dealing with state machines.

## Usage

Implement `IState<C, E, S>` by your base state:

```cs
public interface IState<in TContext, in TEvent, TState>
{
    bool TryCreateNewState(TContext context, TEvent ev, out TState newState);
    void OnExiting(TContext context, TEvent ev, TState newState);
    void OnRemain(TContext context, TEvent ev);
    void OnEntered(TContext context, TEvent ev, TState oldState);
}
```

```cs
MyContext context = …;
MyState initialState = …;
var stateMachine = StateMachine<Event>.Create(context, initialState);

MyEvent ev = …;
_ = stateMachine.TryProcessEvent(ev);
MyState currentState = stateMachine.CurrentState;
```

## License

[![License](https://img.shields.io/github/license/qbit86/machinery)](LICENSE.txt)

The icon is designed by [OpenMoji](https://openmoji.org) — the open-source emoji and icon project.
License: [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).
