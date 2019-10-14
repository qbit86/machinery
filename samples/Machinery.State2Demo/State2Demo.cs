namespace Machinery
{
    using System;
    using System.IO;

    internal enum Event
    {
        None = 0,
        // The same event used for both opening and closing.
        Interact,
        Lock,
        Unlock
    }

    internal sealed class Door
    {
        internal static Door Instance { get; } = new Door();

        private Door() { }
    }

    internal static class State2Demo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
        }
    }
}
