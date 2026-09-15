using Minicraft.Engine.Gameplay;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Rendering.Abstractions;

public interface IRenderer
{
    void Initialize();
    void Render(Camera camera, IEnumerable<ChunkRenderData> chunks, double deltaTime);
    void Resize(int width, int height);
    void Shutdown();
}