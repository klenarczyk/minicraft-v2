using System.Numerics;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Generation;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkStreamer(ChunkStorage chunks)
{
    public int RenderDistance { get; set; } = 4;
    
    public void Update(Vector3 playerPosition)
    {
        var playerChunk = WorldToChunkPosition(playerPosition);
        
        // TODO: Add chunk unloading
        // UnloadFarChunks(playerChunk);
        
        for (int x = -RenderDistance; x <= RenderDistance; x++)
        for (int z = -RenderDistance; z <= RenderDistance; z++)
        {
            var position = new ChunkPosition(playerChunk.X + x, playerChunk.Z + z);
            
            if (chunks.Contains(position)) continue;
            
            LoadChunk(position);
        }
    }
    
    // ---

    private void LoadChunk(ChunkPosition position)
    {
        var chunk = new Chunk(position);
        
        FlatWorldGenerator.Generate(chunk, new BlockId(1));
        
        chunks.Add(chunk);
    }

    private void UnloadFarChunks(ChunkPosition center)
    {
        throw new NotImplementedException();
    }

    private static ChunkPosition WorldToChunkPosition(Vector3 position)
    {
        var chunkX = (int)Math.Floor(position.X / Chunk.SizeX);
        var chunkZ = (int)Math.Floor(position.Z / Chunk.SizeZ);

        return new ChunkPosition(chunkX, chunkZ);
    }
}