using Minicraft.Engine.Gameplay;
using Minicraft.Engine.Input;

namespace Minicraft.Engine;

public sealed class GameEngine
{
    public InputSystem Input { get; } = new();
    public Player Player { get; } = new();
    
    public void Initialize() { }

    public void Update(double deltaTime)
    {
        Player.Update(deltaTime, Input.State);
        
        Input.EndFrame();
    }
    
    public void Shutdown() { }
}