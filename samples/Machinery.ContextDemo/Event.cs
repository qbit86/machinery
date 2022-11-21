namespace Machinery;

internal enum Event
{
    None = 0,

    // The same event used for both opening and closing.
    Interact,
    Lock,
    Unlock
}
