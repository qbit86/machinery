﻿namespace Machinery;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

internal enum EventKind
{
    None = 0,
    Call,
    Move,
    Stop
}

internal readonly struct Event
{
    public Event(EventKind kind, int floor)
    {
        Kind = kind;
        Floor = floor;
    }

    public EventKind Kind { get; }
    public int Floor { get; }

    public override string ToString()
    {
        if (Kind == EventKind.Stop)
            return nameof(EventKind.Stop);

        return $"{Kind}({Floor})";
    }
}

internal abstract class StateBase : IState<TextWriter, Event, StateBase>
{
    internal StateBase(int floor) => Floor = floor;

    internal int Floor { get; }

    public abstract bool TryCreateNewState(TextWriter context, Event ev,
        [NotNullWhen(true)] out StateBase? newState);

    public void OnExiting(TextWriter context, Event ev, StateBase newState)
    {
        const string tag = nameof(StateBase) + "." + nameof(OnExiting);
        context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(newState)}: {newState}");
    }

    public void OnExited(TextWriter context, Event ev, StateBase newState)
    {
        const string tag = nameof(StateBase) + "." + nameof(OnExited);
        context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(newState)}: {newState}");
    }

    public void OnRemain(TextWriter context, Event ev)
    {
        const string tag = nameof(StateBase) + "." + nameof(OnRemain);
        context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}");
    }

    public void OnEntering(TextWriter context, Event ev, StateBase oldState)
    {
        const string tag = nameof(StateBase) + "." + nameof(OnEntering);
        context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(oldState)}: {oldState}");
    }

    public void OnEntered(TextWriter context, Event ev, StateBase oldState)
    {
        const string tag = nameof(StateBase) + "." + nameof(OnEntered);
        context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(oldState)}: {oldState}");
    }

    public sealed override string ToString() => $"{GetType().Name}({Floor})";

    protected static bool Transit(StateBase newState, out StateBase result)
    {
        result = newState;
        return true;
    }

    protected static bool Ignore(out StateBase? result)
    {
        result = default;
        return false;
    }
}

internal sealed class IdleState : StateBase
{
    internal IdleState(int floor) : base(floor) { }

    public override bool TryCreateNewState(TextWriter context, Event ev,
        [NotNullWhen(true)] out StateBase? newState)
    {
        switch (ev.Kind)
        {
            case EventKind.Call:
            case EventKind.Move:
                if (Floor == ev.Floor)
                    return Ignore(out newState);
                return Transit(new MovingState(ev.Floor), out newState);
            default:
                return Ignore(out newState);
        }
    }
}

internal sealed class MovingState : StateBase
{
    internal MovingState(int floor) : base(floor) { }

    public override bool TryCreateNewState(TextWriter context, Event ev,
        [NotNullWhen(true)] out StateBase? newState)
    {
        switch (ev.Kind)
        {
            case EventKind.Stop:
                return Transit(new IdleState(Floor), out newState);
            default:
                return Ignore(out newState);
        }
    }
}

internal static class PolymorphicReferenceStateDemo
{
    private static TextWriter Out => Console.Out;

    private static void Main()
    {
        StateMachine<TextWriter, Event, StateBase> elevator = new(Out, new IdleState(0));

        elevator.TryProcessEvent(new(EventKind.Call, -1));

        Out.WriteLine();
        elevator.TryProcessEvent(new(EventKind.Call, 2));

        Out.WriteLine();
        elevator.TryProcessEvent(new(EventKind.Stop, default));
    }
}
