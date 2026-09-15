using System.Numerics;
using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;
using Minicraft.Engine.World.Meshing;

namespace Minicraft.Engine.World;

public sealed class World
{
    public BlockRegistry Blocks { get; }
    
    public ChunkStorage Chunks { get; }
    public ChunkStreamer Streamer { get; }
    public ChunkMesher Mesher { get; }
    
    public World()
    {
        Blocks = new BlockRegistry();
        Chunks = new ChunkStorage();
        Streamer = new ChunkStreamer(Chunks);
        Mesher = new ChunkMesher(Blocks);
    }
    
    public void Initialize()
    {
        Blocks.Register(new BlockDefinition(BlockId.Air, "air", false));
        Blocks.Register(new BlockDefinition(new BlockId(1), "dirt"));
    }

    public void Update(double deltaTime, Vector3 playerPosition)
    {
        Streamer.Update(playerPosition);
        
        foreach (var chunk in Chunks.LoadedChunks)
        {
            if (chunk.Mesh is not null && !chunk.IsMeshDirty) continue;
            chunk.SetMesh(Mesher.Build(this, chunk));
        }
    }

    public BlockId GetBlock(int worldX, int worldY, int worldZ)
    {
        if (worldY is < 0 or >= Chunk.SizeY)
            return BlockId.Air;

        var chunkX = (int)Math.Floor((double)worldX / Chunk.SizeX);
        var chunkZ = (int)Math.Floor((double)worldZ / Chunk.SizeZ);
        
        int localX = worldX - chunkX * Chunk.SizeX;
        int localZ = worldZ - chunkZ * Chunk.SizeZ;

        var position = new ChunkPosition(chunkX, chunkZ);

        if (!Chunks.TryGet(position, out var chunk))
            return BlockId.Air;
        
        return chunk!.GetBlock(localX, worldY, localZ);
    }
}