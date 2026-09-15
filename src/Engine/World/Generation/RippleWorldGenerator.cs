using Minicraft.Engine.World.Blocks;
using Minicraft.Engine.World.Chunks;

namespace Minicraft.Engine.World.Generation;

public static class RippleWorldGenerator
{
    public static void Generate(Chunk chunk, BlockId block)
    {
        const int amplitude = 5;
        const double frequency = 0.1;
        const int baseline = 10;
        
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            int worldX = chunk.Position.X * Chunk.SizeX + x;
            int worldZ = chunk.Position.Z * Chunk.SizeZ + z;

            double distance = Math.Sqrt(worldX * worldX + worldZ * worldZ);
            
            int height = (int)(amplitude * Math.Cos(frequency * distance)) + baseline;

            for (var y = 0; y < height; y++)
            {
                chunk.SetBlock(x, y, z, block);
            }
        }
    }
}