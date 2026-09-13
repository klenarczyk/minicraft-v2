using System.Numerics;

namespace Minicraft.Engine.Input;

public sealed class InputSystem
{
    public InputBindings Bindings { get; } = new();
    public InputState State { get; } = new();

    public void HandleInput(PhysicalInput input, bool isActive)
    {
        if (!Bindings.TryGetAction(input, out var action)) return;
        State.Set(action, isActive);
    }

    public void AddMouseDelta(Vector2 delta)
    {
        State.AddMouseDelta(delta);
    }

    public void EndFrame()
    {
        State.ClearMouseDelta();
    }
}