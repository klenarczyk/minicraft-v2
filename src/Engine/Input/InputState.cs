using System.Numerics;

namespace Minicraft.Engine.Input;

public sealed class InputState
{
    private readonly HashSet<GameAction> _actions = [];
    public Vector2 MouseDelta { get; private set; }
    
    // ---
    
    public bool IsActive(GameAction action) => _actions.Contains(action);

    public void Set(GameAction action, bool isActive)
    {
        if (isActive) _actions.Add(action);
        else _actions.Remove(action);
    }

    public void AddMouseDelta(Vector2 delta)
    {
        MouseDelta += delta;
    }

    public void ClearMouseDelta()
    {
        MouseDelta = Vector2.Zero;
    }
}