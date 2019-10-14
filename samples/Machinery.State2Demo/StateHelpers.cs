namespace Machinery
{
    internal static class StateHelpers
    {
        internal static bool Transit(IState<Door, Event> newState, out IState<Door, Event> result)
        {
            result = newState;
            return true;
        }

        internal static bool Ignore(out IState<Door, Event> result)
        {
            result = default;
            return false;
        }
    }
}
