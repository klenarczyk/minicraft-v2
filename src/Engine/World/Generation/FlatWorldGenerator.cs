using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Engine.World.Generation;

public static class FlatWorldGenerator
{
    public static BlockId[] Generate(ChunkPosition position, BlockId block)
    {
        var blocks = new BlockId[Chunk.SizeX * Chunk.SizeY * Chunk.SizeZ];
        
        for (var y = 0; y < 16; y++)
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            blocks[Chunk.GetBlockIndex(x, y, z)] = block;
        }

        return blocks;
    }
}