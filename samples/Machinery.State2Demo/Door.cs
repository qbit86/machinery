namespace Machinery
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal sealed partial class Door
    {
        internal Door(TextWriter @out) => Out = @out ?? throw new ArgumentNullException(nameof(@out));

        private TextWriter Out { get; }

        internal static IState<Door, Event> CreateInitialState() => Opened.Instance;

        private void WriteLine(string value, [CallerMemberName] string callerName = "") =>
            Out.WriteLine($"[{nameof(Door)}.{callerName}] {value}");

        private void OnExitingOpened(Event ev, Opened currentState, IState<Door, Event> newState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(newState)}: {newState.GetType().Name}");

        private void OnRemainOpened(Event ev, Opened currentState) =>
            WriteLine($"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}");

        private void OnEnteredOpened(Event ev, Opened currentState, IState<Door, Event> oldState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(oldState)}: {oldState.GetType().Name}");

        private void OnExitingClosed(Event ev, Closed currentState, IState<Door, Event> newState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(newState)}: {newState.GetType().Name}");

        private void OnRemainClosed(Event ev, Closed currentState) =>
            WriteLine($"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}");

        private void OnEnteredClosed(Event ev, Closed currentState, IState<Door, Event> oldState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(oldState)}: {oldState.GetType().Name}");

        private void OnExitingLocked(Event ev, Locked currentState, IState<Door, Event> newState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(newState)}: {newState.GetType().Name}");

        private void OnRemainLocked(Event ev, Locked currentState) =>
            WriteLine($"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}");

        private void OnEnteredLocked(Event ev, Locked currentState, IState<Door, Event> oldState) =>
            WriteLine(
                $"{nameof(ev)}: {ev}, {nameof(currentState)}: {currentState.GetType().Name}, {nameof(oldState)}: {oldState.GetType().Name}");
    }
}
