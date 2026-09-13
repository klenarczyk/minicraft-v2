namespace Minicraft.Engine.Input;

public enum InputDevice
{
    Keyboard,
    Mouse
}

public readonly record struct PhysicalInput(
    InputDevice Device,
    int Code
);