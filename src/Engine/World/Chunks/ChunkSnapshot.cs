using Minicraft.Engine.World.Blocks;

namespace Minicraft.Engine.World.Chunks;

public sealed class ChunkSnapshot(ChunkPosition position, BlockId[] blocks)
{
    public ChunkPosition Position { get; } = position;

    private readonly BlockId[] _blocks = (BlockId[])blocks.Clone();
    public IReadOnlyList<BlockId> Blocks => _blocks;

    public BlockId GetBlock(int x, int y, int z)
    {
        return _blocks[Chunk.GetBlockIndex(x, y, z)];
    }
}