using Minicraft.Engine.Gameplay;
using Minicraft.Engine.Geometry;
using Minicraft.Engine.World.Meshing;

namespace Minicraft.Rendering;

public interface IRenderer
{
    void Initialize();
    void Render(Camera camera, MeshData worldMesh, double deltaTime);
    void Resize(int width, int height);
    void Shutdown();
}