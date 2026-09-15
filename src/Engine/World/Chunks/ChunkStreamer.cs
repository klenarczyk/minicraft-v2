using System.Numerics;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Generation;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkStreamer(ChunkStorage chunks)
{
    public int LoadDistance { get; set; } = 5;
    
    public void Update(Vector3 playerPosition)
    {
        var playerChunk = WorldToChunkPosition(playerPosition);
        
        UnloadFarChunks(playerChunk);
        
        for (int x = -LoadDistance; x <= LoadDistance; x++)
        for (int z = -LoadDistance; z <= LoadDistance; z++)
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
        
        RippleWorldGenerator.Generate(chunk, new BlockId(1));
        
        chunks.Add(chunk);
        
        chunk.MarkMeshDirty();

        foreach (var offset in Neighbors)
        {
            var neighPos = new ChunkPosition(
                chunk.Position.X + offset.X,
                chunk.Position.Z + offset.Z
            );
            
            if (chunks.TryGet(neighPos, out var neigh))
                neigh!.MarkMeshDirty();
        }
    }

    private void UnloadFarChunks(ChunkPosition center)
    {
        foreach (var chunk in chunks.LoadedChunks.ToArray())
        {
            var pos = chunk.Position;

            if (Math.Abs(pos.X - center.X) > LoadDistance ||
                Math.Abs(pos.Z - center.Z) > LoadDistance)
            {
                // Chunk saving will be added later on
                chunks.Remove(pos, out _);
            }
        }
    }

    private static ChunkPosition WorldToChunkPosition(Vector3 position)
    {
        var chunkX = (int)Math.Floor(position.X / Chunk.SizeX);
        var chunkZ = (int)Math.Floor(position.Z / Chunk.SizeZ);

        return new ChunkPosition(chunkX, chunkZ);
    }

    private static readonly ChunkPosition[] Neighbors =
    [
        new (-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1)
    ];
}