﻿namespace Machinery
{
    using System;
    using System.IO;

    internal enum State
    {
        None = 0,
        IdleDown,
        IdleUp,
        MovingDown,
        MovingUp
    }

    internal enum Event
    {
        None = 0,
        CallDown,
        CallUp,
        Move,
        Stop
    }

    internal readonly struct ElevatorPolicy : IStatePolicy<State, Event>
    {
        private TextWriter Out { get; }

        public ElevatorPolicy(TextWriter @out)
        {
            Out = @out;
        }

        public bool TryCreateNewState(State currentState, Event ev, out State newState)
        {
            switch (currentState)
            {
                case State.IdleDown:
                    switch (ev)
                    {
                        case Event.CallDown: return Ignore(out newState);
                        case Event.CallUp: return Transit(State.MovingUp, out newState);
                        case Event.Move: return Transit(State.MovingUp, out newState);
                        case Event.Stop: return Ignore(out newState);
                        default: return Ignore(out newState);
                    }
                case State.IdleUp:
                    throw new NotImplementedException();
                case State.MovingDown:
                    throw new NotImplementedException();
                case State.MovingUp:
                    throw new NotImplementedException();
                default:
                    return Ignore(out newState);
            }
        }

        public void OnExiting(State currentState, Event ev, State newState)
        {
            Out.WriteLine($"{nameof(OnExiting)}({ev}): {currentState} -> {newState}");
        }

        public void OnEntered(State currentState, Event ev, State oldState)
        {
            Out.WriteLine($"{nameof(OnEntered)}({ev}): {oldState} -> {currentState}");
        }

        public void DisposeState(State currentState, Event ev, State stateToDispose)
        {
        }

        private bool Transit(State newState, out State result)
        {
            result = newState;
            return true;
        }

        private bool Ignore(out State result)
        {
            result = default;
            return false;
        }
    }

    internal static class ElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            StateMachine<State, Event, ElevatorPolicy> elevator =
                StateMachine<Event>.Create(State.IdleDown, new ElevatorPolicy(Out));
            Out.WriteLine($"{nameof(elevator.CurrentState)}: {elevator.CurrentState}");
            Out.WriteLine();

            elevator.ProcessEvent(Event.CallDown);
            Out.WriteLine($"{nameof(elevator.CurrentState)}: {elevator.CurrentState}");
            Out.WriteLine();

            elevator.ProcessEvent(Event.CallUp);
            Out.WriteLine($"{nameof(elevator.CurrentState)}: {elevator.CurrentState}");
            Out.WriteLine();
        }

        private static void ProcessEvent(this StateMachine<State, Event, ElevatorPolicy> elevator, Event ev)
        {
            Out.WriteLine($"{ev}");
            elevator.Process(ev);
        }
    }
}
