using Minicraft.Engine.Gameplay;
using Minicraft.Engine.Input;

namespace Minicraft.Engine;

public sealed class GameEngine
{
    public InputSystem Input { get; } = new();
    public World.World World { get; } = new();
    public Player Player { get; } = new();
    
    public void Initialize()
    {
        World.Initialize();
    }

    public void Update(double deltaTime)
    {
        Player.Update(deltaTime, Input.State);
        World.Update(deltaTime, Player.Camera.Position);
        
        Input.EndFrame();
    }
    
    public void Shutdown() { }
}