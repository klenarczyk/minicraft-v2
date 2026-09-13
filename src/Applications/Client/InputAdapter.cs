using Minicraft.Engine.Input;
using Silk.NET.Input;

namespace Minicraft.Applications.Client;

public static class InputAdapter
{
    public static PhysicalInput ToPhysicalInput(Key key)
    {
        return new PhysicalInput(InputDevice.Keyboard, (int)key);
    }

    public static PhysicalInput ToPhysicalInput(MouseButton button)
    {
        return new PhysicalInput(InputDevice.Mouse, (int)button);
    }
}