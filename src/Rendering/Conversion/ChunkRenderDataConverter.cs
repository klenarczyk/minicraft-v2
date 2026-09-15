using Minicraft.Engine.World.Chunks;
using Minicraft.Rendering.Abstractions;

namespace Minicraft.Rendering.Conversion;

public static class ChunkRenderDataConverter
{
    public static IEnumerable<ChunkRenderData> Convert(IEnumerable<Chunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            if (chunk.Mesh is null) continue;
            
            yield return new ChunkRenderData(chunk.Position, chunk.Mesh, chunk.MeshVersion);
        }
    }
}