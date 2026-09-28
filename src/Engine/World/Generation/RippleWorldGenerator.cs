using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Engine.World.Generation;

public static class RippleWorldGenerator
{
    public static BlockId[] Generate(ChunkPosition position, BlockId block)
    {
        var blocks = new BlockId[Chunk.SizeX * Chunk.SizeY * Chunk.SizeZ];
        
        const int amplitude = 5;
        const double frequency = 0.1;
        const int baseline = 10;
        
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            int worldX = position.X * Chunk.SizeX + x;
            int worldZ = position.Z * Chunk.SizeZ + z;

            double distance = Math.Sqrt(worldX * worldX + worldZ * worldZ);
            
            int height = (int)(amplitude * Math.Cos(frequency * distance)) + baseline;

            for (var y = 0; y < height; y++)
            {
                blocks[Chunk.GetBlockIndex(x, y, z)] = block;
            }
        }

        return blocks;
    }
}