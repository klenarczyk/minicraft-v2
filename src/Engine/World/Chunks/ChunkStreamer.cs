using System.Numerics;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkStreamer
{
    public int LoadDistance { get; set; } = 8;

    private readonly HashSet<ChunkPosition> _desiredChunks = [];
    public IReadOnlySet<ChunkPosition> DesiredChunks => _desiredChunks;
    
    public void Update(Vector3 playerPosition)
    {
        var playerChunk = WorldToChunkPosition(playerPosition);
        
        _desiredChunks.Clear();
        
        for (int x = -LoadDistance; x <= LoadDistance; x++)
        for (int z = -LoadDistance; z <= LoadDistance; z++)
        {
            var position = new ChunkPosition(playerChunk.X + x, playerChunk.Z + z);
            _desiredChunks.Add(position);
        }
    }
    
    // ---
    
    private static ChunkPosition WorldToChunkPosition(Vector3 position)
    {
        var chunkX = (int)Math.Floor(position.X / Chunk.SizeX);
        var chunkZ = (int)Math.Floor(position.Z / Chunk.SizeZ);

        return new ChunkPosition(chunkX, chunkZ);
    }
}