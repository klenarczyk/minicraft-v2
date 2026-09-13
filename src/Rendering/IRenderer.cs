using Minicraft.Engine.Gameplay;

namespace Minicraft.Rendering;

public interface IRenderer
{
    void Initialize();
    void Render(Camera camera, double deltaTime);
    void Resize(int width, int height);
    void Shutdown();
}