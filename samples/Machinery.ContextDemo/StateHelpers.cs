namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;

    internal static class StateHelpers
    {
        internal static bool Transit(State newState, out State result)
        {
            result = newState;
            return true;
        }

        internal static bool Ignore([NotNullWhen(true)] out State? result)
        {
            result = default;
            return false;
        }
    }
}
