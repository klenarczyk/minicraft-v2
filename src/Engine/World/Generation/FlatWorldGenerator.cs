namespace Minicraft.Engine.World.Generation;

public static class FlatWorldGenerator
{
    public static void Generate(Chunk chunk, BlockId block)
    {
        for (var y = 0; y < 16; y++)
        for (var z = 0; z < Chunk.SizeZ; z++)
        for (var x = 0; x < Chunk.SizeX; x++)
        {
            chunk.SetBlock(x, y, z, block);
        }
    }
}